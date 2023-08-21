using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/status-orderings")]
    [ApiController]
    public class StatusOrderingController : ControllerBase
    {
        private readonly IStatusOrderingRepository _I;
        public StatusOrderingController(IStatusOrderingRepository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return StatusCode(200, new ResponseAPI<List<StatusOrdering>>
            {
                Message = "Success",
                Result = await _I.GetAll()
            }) ;
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new(StatusOrdering entity)
        {
            try
            {
            return StatusCode(201, new ResponseAPI<StatusOrdering>
            {
                Message = "Created",
                Result = await _I.Insert(entity)
            }.Format());
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, StatusOrdering entity)
        {
            try
            {
                entity.Id = id;
                var IsSuccess= await _I.Update(entity);
                if(IsSuccess)
                    return StatusCode(200, new ResponseAPI<string>
                    {
                        Message = "Success"
                    }.Format());
                return StatusCode(404, new ResponseAPI<string>
                {
                    Message = "Failed"
                }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var IsSuccess = await _I.DeletebyId(id);
                return StatusCode(200, new ResponseAPI<string>
                {
                    Message = IsSuccess ? "Success" : "Failed"
                }.Format());
            }
            catch 
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
    }
}
