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
        private readonly IAccountRepository _I;
        private readonly ICartRepository _Cart;
        public UsersController(IAccountRepository i, ICartRepository cart)
        {
            _I = i;
            _Cart = cart;
        }
        [HttpGet]
        public async Task<ActionResult> GETALL(string? keywords,string? type, DateTime? startdate, DateTime? enddate, int? role, bool? gender, string? sort, int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<PagingResult<Account>>
                {
                    Result = await _I.GetAll(keywords,type, startdate, enddate, role, gender, pageIndex, pageSize, sort ?? "date"),
                    Message = "Success"
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            try
            {
                var rs = await _I.GetbyId(id);
                return StatusCode(200, new ResponseAPI<Account>
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
        public async Task<ActionResult> ADD([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var a = await _I.Insert(new ImageUpload<Account>
                {
                    data = JsonConvert.DeserializeObject<Account>(data),
                    images = images
                });
                await _Cart.Create(a.Id!);
                return StatusCode(201, new ResponseAPI<string> { Message = "Create Success" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string> { Message = ex.Message }.Format());
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UPDATE([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var account = JsonConvert.DeserializeObject<Account>(data);
                account!.Id = this.HttpContext.GetRouteValue("id")!.ToString();
                await _I.Update(new ImageUpload<Account>
                {
                    data = account,
                    images = images
                });
                return StatusCode(200, new ResponseAPI<string> { Message = "Update Successfull" }.Format());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DELETE(string id)
        {
            try
            {
                string result = await _I.DeletebyId(id) == true ? "deleted" : "This item is does not exits";
                return StatusCode(200, new ResponseAPI<string> { Message = result }.Format());
            }
            catch
            {
                return NotFound();
            }
        }

    }
}





