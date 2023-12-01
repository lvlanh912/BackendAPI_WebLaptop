using Amazon.Runtime.Internal;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountRepository _i;
        private readonly ICartRepository _cart;
        private readonly IAuthenticationRepository _auth;
        public UsersController(IAccountRepository i, ICartRepository cart,IAuthenticationRepository auth)
        {
            _i = i;
            _cart = cart;
            _auth = auth;
        }
        //[Authorize(Roles = "Admin")]//theo role
        [HttpGet]
        public async Task<ActionResult> Getall(string? keywords,string? type, DateTime? startdate, DateTime? enddate, int? role, bool? gender, string? sort, int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Account>>
                {
                    Result = await _i.GetAll(keywords,type, startdate, enddate, role, gender, pageIndex, pageSize, sort ?? "date"),
                    Message = "Success"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //admin
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            try
            {
                var rs = await _i.GetbyId(id);
                return StatusCode(200, new ResponseApi<Account>
                {
                    Result = rs,
                    Message = rs != null ? "Success" : "Invalid user"
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        //admin
        [HttpGet("total-orders")]
        public async Task<ActionResult> GetTotalOrder (string id)
        {
            try
            {
                var rs = await _i.GetTotalOrder(id);
                return StatusCode(200, new ResponseApi<int>
                {
                    Result = rs
                    
                });
            }
            catch
            {
                return BadRequest(new ResponseApi<bool> { Result=false});
            }
        }
        //admin
        [HttpGet("total-comments")]
        public async Task<ActionResult> GetTotalComment(string id)
        {
            try
            {
                var rs = await _i.GetTotalComment(id);
                return StatusCode(200, new ResponseApi<int>
                {
                    Result = rs

                });
            }
            catch
            {
                return BadRequest(new ResponseApi<bool> { Result = false });
            }
        }
        //admin
        [HttpPost]
        public async Task<ActionResult> Add([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var a = await _i.Insert(new ImageUpload<Account>
                {
                    Data = JsonConvert.DeserializeObject<Account>(data),
                    Images = images
                });
                await _cart.Create(a.Id!);
                return StatusCode(201, new ResponseApi<string> { Message = "Create Success" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        //admin
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var account = JsonConvert.DeserializeObject<Account>(data);
                account!.Id = this.HttpContext.GetRouteValue("id")!.ToString();
                await _i.Update(new ImageUpload<Account>
                {
                    Data = account,
                    Images = images
                });
                return StatusCode(200, new ResponseApi<string> { Message = "Update Successfull" }.Format());
            }
            catch(Exception ex)
            {
                return BadRequest( new ResponseApi<string> { Message=ex.Message});
            }
        }
        //admin
        //[Authorize(Roles = "admin")]//theo role
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                string result = await _i.DeletebyId(id) == true ? "deleted" : "This item is does not exist";
                return StatusCode(200, new ResponseApi<string> { Message = result }.Format());
            }
            catch
            {
                return NotFound();
            }
        }
        //admin
        [HttpGet("sum-create")]
        public async Task<ActionResult> GetTotalCreate(DateTime? start,DateTime? end)
        {
            try
            {
                return StatusCode(200, new ResponseApi<long>
                {
                    Result = await _i.GetTotalCreatebyTime(start ?? DateTime.Today, end ?? DateTime.Now),
                    Message="Success"

            });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false,Message=ex.Message });
            }
        }
        //no-role
        [HttpPost("sign-up")]
        public async Task<ActionResult> SignUp(Account account)
        {
            try
            {
                var result= await _i.Insert(new ImageUpload<Account> { Data = account });
                return StatusCode(201, new ResponseApi<Account> { Message = "Đăng ký thành công" }.Format());
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng ký thất bại: {ex.Message}" });
            }
        }
        [HttpPost("sign-in")]
        public async Task<ActionResult> SignIn(Account account)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                var result = await _auth.Createtoken(account);
                return StatusCode(201, new ResponseApi<string> { Message = "Đăng nhập thành công",Result=result });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = $"Đăng nhập thất bại: {ex.Message}" });
            }
        }
        
        [Authorize]
        [ServiceFilter(typeof(SessionAuthor))]
        //thêm 1 lớp kiểm tra database có token này không
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                return StatusCode(200, new ResponseApi<string> { Message = "thành công" }.Format());
            }
            catch
            {
                return BadRequest();
            }
        }
        //no-role
    }

}





