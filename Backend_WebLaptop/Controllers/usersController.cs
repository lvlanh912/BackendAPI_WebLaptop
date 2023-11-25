using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
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
        public UsersController(IAccountRepository i, ICartRepository cart)
        {
            _i = i;
            _cart = cart;
        }
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

    }
}





