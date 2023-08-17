using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;
 using System.Linq;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountResposytory _I;
        public UsersController(IAccountResposytory i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> GETALL(string? keywords, int pageIndex, int pageSize)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<PagingResult<Account>>
                {
                    Result = await _I.GetAll(keywords, pageIndex, pageSize),
                    Message = "Success"
                }.Format());
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
                    Message = rs != null? "Success":"Invalid user"
                }.Format()) ;
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult> ADD([FromForm] ImageUpload? image, [FromForm] Account account)
        {
            try
            {
                await _I.Insert(account, image!);
                return StatusCode(201, new ResponseAPI<string> { Message = "Create Success" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string> { Message = ex.Message }.Format());
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UPDATE([FromForm] ImageUpload? image, [FromForm] Account account, string id)
        {
            try
            {
                account.Id=id;
                await _I.Update(account, image);
                return StatusCode(200, new ResponseAPI<string> { Message = "Update Successfull" }.Format());
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DELETE(string id)
        {
            try
            {
                string result = await _I.DeletebyId(id)==true?"deleted":"This item is does not exits";
                return StatusCode(200, new ResponseAPI<string> { Message=result}.Format());
            }
            catch
            {
                return NotFound();
            }
        }
       
    }
}





