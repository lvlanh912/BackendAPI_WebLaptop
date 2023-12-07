using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountRepository _i;
        private readonly IAuthenticationRepository _auth;
        public AuthController(IAccountRepository i, IAuthenticationRepository auth)
        {
            _i = i;
            _auth = auth;
        }
        //no-role
        [HttpPost("sign-up")]
        public async Task<ActionResult> SignUp(Account account)
        {
            try
            {
                var result = await _i.Insert(new ImageUpload<Account> { Data = account });
                return StatusCode(201, new ResponseApi<Account> { Message = "Đăng ký thành công" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng ký thất bại: {ex.Message}" });
            }
        }
        [HttpPost("sign-in")]
        public async Task<ActionResult> SignIn(Account account)
        {
            try
            {

                var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString(); ;
                var browser = HttpContext.Request.Headers.UserAgent;
                var result = await _auth.Createtoken(account, browser, ip ?? "");
                return StatusCode(201, new ResponseApi<string> { Message = "Đăng nhập thành công", Result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng nhập thất bại: {ex.Message}" });
            }
        }

    }
}
