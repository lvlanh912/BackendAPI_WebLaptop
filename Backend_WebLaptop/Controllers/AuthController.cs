using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
                var result = await _auth.CreatetokenForUser(account, browser, ip ?? "", 1);
                return StatusCode(201, new ResponseApi<string> { Message = "Đăng nhập thành công", Result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng nhập thất bại: {ex.Message}" });
            }
        }
        [HttpPost("admin/sign-in")]
        public async Task<ActionResult> SignInAdmin(Account account)
        {
            try
            {

                var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString(); ;
                var browser = HttpContext.Request.Headers.UserAgent;
                var result = await _auth.CreatetokenForUser(account, browser, ip ?? "", 2);
                return StatusCode(201, new ResponseApi<string> { Message = "Đăng nhập thành công", Result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng nhập thất bại: {ex.Message}" });
            }
        }
        [HttpGet("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            try
            {
                return StatusCode(200, new ResponseApi<string> { Message = "Đường dẫn đặt lại mật khẩu đã được gửi về email của bạn",
                    Result =await _auth.CreateTokenForResetPassword(email) });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message =  ex.Message });
            }
        }
        [HttpGet("reset-password")]
        public async Task<ActionResult> ResetPassword(string token)
        {
            try
            {
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = "Thành công",
                    Result = await _auth.ResetPassword(token)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }

    }
}
