using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UAParser;

namespace Backend_WebLaptop.Respository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IMongoCollection<Account>? _accounts;
        private readonly ISessionRepository _session;
        private readonly string? _sercretKey;
        public AuthenticationRepository(IDatabaseService database, ISessionRepository session, IOptions<AuthenticationConfig> authenticationConfig)
        {
            _session = session;
            _accounts = database.Get_Accounts_Collection();
            _sercretKey = authenticationConfig.Value.SecretKey;
        }

        

        Task<bool> IAuthenticationRepository.CheckValidToken(string jwttoken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Createtoken(Account entity, string browser, string ipaddress)
        {
            var account = await _accounts.FindSync(e => e.Username == entity.Username && e.Password == entity.Password).FirstOrDefaultAsync() 
                ?? throw new Exception("Sai tên đăng nhập hoặc mật khẩu");
            if (account.Role == 0)
                throw new Exception("Tài khoản bị vô hiệu hoá");
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
    }
}
