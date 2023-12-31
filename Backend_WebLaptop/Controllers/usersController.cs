﻿using Amazon.Runtime.Internal;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using UAParser;
using static System.Net.Mime.MediaTypeNames;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountRepository _i;
        private readonly ICartRepository _cart;
        private readonly ISessionRepository _session;
        
        public UsersController(IAccountRepository i, ICartRepository cart,ISessionRepository session)
        {
            _i = i;
            _cart = cart;
            _session = session;
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("isAdmin")]
        public ActionResult CheckAdmin()
        {
            return StatusCode(200, new ResponseApi<bool> { Result = true });
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
       
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                var result = await _i.GetbyId(accounId);
                return StatusCode(200, new ResponseApi<Account> { Message = "thành công",Result=result});
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPatch("update-avatar")]
        public async Task<ActionResult> UpdateAvatar( IFormFile image)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                var entity = new ImageUpload<Account> { 
                    Data = new Account { Id = accounId },
                    Images = new List<IFormFile> { image }
                };
                 await _i.UpdateImage(entity);
                return StatusCode(200, new ResponseApi<Account> { Message = "thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPatch("update-infor")]
        public async Task<ActionResult> UpdateInfor(Account account)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                account.Id = accounId;
                await _i.Updateinfor(account);
                return StatusCode(200, new ResponseApi<Account> { Message = "thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("session")]
        public async Task<ActionResult> GetSession()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(200, new ResponseApi<List<Session>> { 
                    Message = "thành công",
                    Result = await _session.GetAllSession(accounId)
            });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpDelete("delete-session/{id}")]
        public async Task<ActionResult> DeleteSession(string id)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                await _session.RemoveSession(id,accounId);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Message = "thành công",
                    Result=true
                    
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        
        [Authorize]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var jwt = HttpContext.Request.Headers.Authorization;
                await _session.RemoveSessionByValue(jwt);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Message = "thành công",
                    Result = true

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [HttpGet("userinfor")]
        public async Task<ActionResult> GetUserInfor(string accountId)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Account>
                {
                    Message = "thành công",
                    Result = await _i.GetPublicInfor(accountId)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword( dynamic Json)
        {
            try
            {
                var password=string.Empty;
                var newpassword=string.Empty;
                //kiểm tra loại dữ liệu nhận
                if (Json.ValueKind != JsonValueKind.Object)
                    throw new Exception("Invalid data");

                if (Json.TryGetProperty("password", out JsonElement passwordElement))
                    password = passwordElement.GetString()??throw new Exception("Invalid data");

                if(Json.TryGetProperty("newpassword", out JsonElement newpasswordElement))
                    newpassword = newpasswordElement.GetString() ?? throw new Exception("Invalid data");

               
                if(!string.IsNullOrEmpty(password)&& !string.IsNullOrEmpty(newpassword))
                {
                    var accounId = HttpContext.User.FindFirst("Id")!.Value;
                    await _i.ChangePassword(accounId, password, newpassword);
                    return StatusCode(200, new ResponseApi<bool>
                    {
                        Message = "thành công",
                        Result = true

                    });
                }
                throw new Exception("Invalid data");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
    }
}







