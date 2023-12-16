using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

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
        public async Task<ActionResult> SignUp(Account account,int otp)
        {
            try
            {
                if(await _auth.ConfirmEmail(account.Email, otp))
                {
                    var result = await _i.Insert(new ImageUpload<Account> { Data = account });
                    return StatusCode(201, new ResponseApi<Account> { Message = "Đăng ký thành công" }.Format());
                }
                throw new Exception("Mã xác nhận email không hợp lệ ");
                
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
                var ipAddress = IPAddress.Parse(Request.HttpContext.Request.Headers["X-Forwarded-For"]);
               
                var browser = HttpContext.Request.Headers.UserAgent;
                var result = await _auth.CreatetokenForUser(account, browser, ipAddress!=null?ipAddress.ToString():"127.0.0.1", 1);
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
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromForm]string email)
        {
            try
            {
                await _auth.SendLinkResetPassword(email);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Message = "Đường dẫn đặt lại mật khẩu đã được gửi về email của bạn",
                    Result = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message =  ex.Message });
            }
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromForm] string accesstoken)
        {
            try
            {
                await _auth.ResetPassword(accesstoken);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Message = "Thành công",
                    Result = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        [HttpPost("get-otp")]
        public async Task<ActionResult> ConfirmEmail([FromForm] string email)
        {
            try
            {
                await _auth.GetConfirmEmail(email);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Message = "Thành công",
                    Result = true 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }

    }
}
