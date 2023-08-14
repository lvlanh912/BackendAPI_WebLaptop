using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;
 using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly IAccountsRespository _I;
        public usersController(IAccountsRespository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult<ResponseAPI<PagingResult<Account>>>> GETALL()
        {
            return new ResponseAPI<PagingResult<Account>>
            {
                Result = await _I.GetAll(null, 1, 5),
                Status = "Success"
            };
        }
        [HttpPost]
        public async Task<ActionResult> ADD(Account account)
        {
            try
            {
                await _I.Insert(account);
                return StatusCode(201,new ResponseAPI<string> { Message = "Create Success" }.Format());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Benchmark]
        public async Task<ActionResult> UPDATE(Account account)
        {
            try
            {
                await _I.Update(account);
                return StatusCode(200, new ResponseAPI<string> { Message = "Update Successfull" }.Format());
            }
            catch
            {
                return NotFound();
            }
        }
    }
}





