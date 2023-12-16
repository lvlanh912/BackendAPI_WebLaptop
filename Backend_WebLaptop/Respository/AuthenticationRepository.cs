using Amazon.Runtime;
using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Timers;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UAParser;
using Timer = System.Timers.Timer;
using System.Threading;
using Amazon.Runtime.Internal;

namespace Backend_WebLaptop.Respository
{
    class CodeBag
    {
        public DateTime Timeout { get; set; }
        public int OTP { get; set; }
    }
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly static IDictionary<string, bool> TimesUseJWT = new Dictionary<string, bool>();
        private readonly static IDictionary<string, CodeBag> emailPendingConfirm = new Dictionary<string, CodeBag>();
        private readonly static Timer timer = new() { Interval = 600 };//10 phút
        private readonly IMongoCollection<Account>? _accounts;
        private readonly ISessionRepository _session;
        private readonly string? _sercretKey;
        private readonly IEmailSendRepository _email;
        private readonly FrontendConfig _frontendConfig;
        public AuthenticationRepository(IDatabaseService database, ISessionRepository session, IEmailSendRepository email,
            IOptions<AuthenticationConfig> authenticationConfig , IOptions<FrontendConfig> Fronendconfig)
        {
            _session = session;
            _accounts = database.Get_Accounts_Collection();
            _sercretKey = authenticationConfig.Value.SecretKey;
            _email = email;
            _frontendConfig = Fronendconfig.Value;
            timer.Elapsed += AutoCleanEmailPending!;
        }

        public async Task<string> CreatetokenForUser(Account entity, string browser, string ipaddress,int role)
        {
            var account = await _accounts.FindSync(e => e.Username == entity.Username && e.Password == entity.Password &&e.Role==role).FirstOrDefaultAsync() 
                ?? throw new Exception("Sai tên đăng nhập hoặc mật khẩu");
          
            //cấp token
            //lấy thông tin để mã hoá vào token
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("username", account.Username!),
                        new Claim("Fullname",account.Fullname!),
                        new Claim("Id",account.Id!),
                        new Claim(ClaimTypes.Role,account.Role==2?"Admin":"Member")//thêm role vào đây
                    };
            //tạo token
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sercretKey??throw new Exception("No SercretKey in setting.json")));
            var signIn = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            //khởi tạo token
            var token = new JwtSecurityToken(
              null,
              null,
              claims,
              expires: DateTime.UtcNow.AddYears(1),//thời hạn 1 năm
              signingCredentials: signIn);
            var result = new JwtSecurityTokenHandler().WriteToken(token);

            //Thêm thông tin phiên đăng nhập
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(browser);

            if (clientInfo != null)
            {
                browser = clientInfo.UA + " on " + clientInfo.OS +$"({ipaddress})";
            }
            await _session.Insert(new Session { AccounId = account.Id, IpAddress = browser, Value = result });
            return result;
        }

        public async Task SendLinkResetPassword(string email)
        {
            var account = await _accounts.FindSync(e => e.Email == email&&e.Role!=2).FirstOrDefaultAsync();
            if(account is not null)
            {
                var claims = new Claim[]
                {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("username", account.Username!),
                        new Claim("Email",account.Email!),
                        new Claim("Id",account.Id!)     
                };
                var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sercretKey ?? throw new Exception("No SercretKey in setting.json")));
                var signIn = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            
                var token = new JwtSecurityToken(
                      null,
                      null,
                      claims,
                      expires: DateTime.UtcNow.AddMinutes(10),//thời hạn 10 phút
                      signingCredentials: signIn);
                string rs= new JwtSecurityTokenHandler().WriteToken(token);
                TimesUseJWT[account.Id!] = true;//thêm lượt sửa dụng, ghi đè nếu đã có
                await _email.Sendmail(account.Email!, "Đặt lại mật khẩu của bạn",$"Xin chào! Đây là đường dẫn đặt lại mật khẩu của bạn, sẽ hết hạn sau 10 phút {_frontendConfig.Host+ _frontendConfig.ForgotPasswordPage+rs}");
            }
            else

                throw new Exception("Không tồn tại tài khoản với email này");
            
        }

        public async Task ResetPassword(string accessToken)
        {
    
                var accountId = ValidateToken(accessToken);
                if(TimesUseJWT.TryGetValue(accountId, out var times))
                {
                    //Đổi password
                    var password = RandomPassword();
                    var Update = Builders<Account>.Update.Set(e => e.Password, password);
                    var account= await _accounts.FindOneAndUpdateAsync(e => e.Id == accountId, Update);
                    TimesUseJWT.Remove(accountId);
                    //gửi email
                    await _email.Sendmail(account.Email!, "Mật khẩu đã thay đổi", $"Xin chào {account.Fullname} ({account.Username}) mật khẩu mới của bạn là : {password}");
                }
                else
                {
                    throw new Exception("Đã được sử dụng trước đó");
                }
              
        }

        static string RandomPassword()
        {
            var size = new Random().Next(8, 12);
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";//72 character
            var passwordChar = new char[size];
            for (int i = 0; i < size; i++)
                passwordChar[i] = validChars[new Random().Next(0, 71)];
            return new string(passwordChar);
        }
        string ValidateToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_sercretKey!)),
                ValidateIssuer = false, // Set to true if you want to validate the issuer
                ValidateAudience = false, // Set to true if you want to validate the audience
                ClockSkew = TimeSpan.Zero // You can adjust the acceptable clock skew here
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            var userId = principal.FindFirst("Id")!.Value;
            //var email = principal.FindFirst("Email")!.Value;
           
            return userId;
        }

        public async Task GetConfirmEmail(string email)
        {
            //kiểm tra có email trước đó không,nếu có và đã hết hạn thì thay thế
            if (emailPendingConfirm.TryGetValue(email, out var time))
                if (time.Timeout < DateTime.Now)
                    emailPendingConfirm.Remove(email);
                else
                    throw new Exception("Mã đã được gửi trước đó hãy kiểm tra email");
            var OTP = new Random().Next(100000, 999999);
            await _email.Sendmail(email, "Xác thực đăng ký tài khoản - Ecomerce Website Lvlanh", $"Mã xác nhận của bạn là: {OTP} (Hiệu lực 5 phút)");
            emailPendingConfirm.Add(email, new CodeBag { OTP=OTP,Timeout=DateTime.Now.AddMinutes(5)});
            //chạy timer
            if (!timer.Enabled)
                timer.Enabled = true;

        }
        //tự động làm sạch danh sách email chờ đã hết hạn
        void AutoCleanEmailPending(object sender, EventArgs e)
        {
            lock (emailPendingConfirm)
            {
                //nếu có danh sách
                if (emailPendingConfirm.Count > 0)
                    foreach (var item in emailPendingConfirm)
                    {
                        if (item.Value.Timeout <= DateTime.Now)
                            emailPendingConfirm.Remove(item);
                    }
                else
                    timer.Enabled = false;
            }
        }

        public Task<bool> ConfirmEmail(string? email, int otp)
        {
            if (email is null)
                throw new Exception("Vui lòng nhập địa chỉ email");
            lock (emailPendingConfirm)
            {
               if(emailPendingConfirm.TryGetValue(email,out var value))
                    return Task.FromResult(otp == value.OTP);
            }
            return Task.FromResult(false);
        }


    }
}
