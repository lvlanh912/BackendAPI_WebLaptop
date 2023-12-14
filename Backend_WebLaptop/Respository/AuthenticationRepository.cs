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
using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UAParser;

namespace Backend_WebLaptop.Respository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly static IDictionary<string, bool> TimesUseJWT = new Dictionary<string, bool>();
        private readonly IMongoCollection<Account>? _accounts;
        private readonly ISessionRepository _session;
        private readonly string? _sercretKey;
        public AuthenticationRepository(IDatabaseService database, ISessionRepository session, IOptions<AuthenticationConfig> authenticationConfig)
        {
            _session = session;
            _accounts = database.Get_Accounts_Collection();
            _sercretKey = authenticationConfig.Value.SecretKey;
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

        public async Task<string> CreateTokenForResetPassword(string email)
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
                      expires: DateTime.UtcNow.AddMinutes(1),//thời hạn 10 phút
                      signingCredentials: signIn);
                string rs= new JwtSecurityTokenHandler().WriteToken(token);
                TimesUseJWT[account.Id!] = true;//thêm lượt sửa dụng, ghi đè nếu đã có
                return rs;
            }
            throw new Exception("Không tồn tại tài khoản với email này");
        }

        public async Task<string> ResetPassword(string accessToken)
        {
            try
            {
                var accountId = ValidateToken(accessToken);
                if(TimesUseJWT.TryGetValue(accountId, out var times))
                {
                    //Đổi password
                    var password = RandomPassword();
                    var Update = Builders<Account>.Update.Set(e => e.Password, password);
                    await _accounts.UpdateOneAsync(e => e.Id == accountId, Update);
                    TimesUseJWT.Remove(accountId);
                    //gửi email
                    return password;
                }
                throw new Exception("Đã được sử dụng");
                
            }
            catch
            {
                throw new Exception("Không hợp lệ");
            }
        }
        string RandomPassword()
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
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            var userId = principal.FindFirst("Id")!.Value;
            var email = principal.FindFirst("Email")!.Value;
            /*long unixTimestamp = ConvertToUnixTimestamp(DateTime.Now);
            if (Convert.ToInt64(exp) < unixTimestamp)
            {
                throw new Exception("Link đã hết hạn");
            }
            var email = claim.Claims.First(c => c.Type == "Email").Value;*/
            return userId;
        }


    }
}
