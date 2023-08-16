using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/{ProductId}/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _I;
        public CommentController(ICommentRepository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> Get_Commnets_byProductId(int pageindex,int size)
        {
            try
            {
               string ProductId = this.RouteData.Values["ProductId"]!.ToString()!;
                return StatusCode(200, new ResponseAPI<PagingResult<Comment>>
            {
                Message = "Success",
                Result = await _I.GetAll(pageindex, ProductId, size)
            }) ;
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new( Comment entity)
        {
            try
            {
                string ProductId = this.RouteData.Values["ProductId"]!.ToString()!;
                entity.ProductId = ProductId;
                return StatusCode(201, new ResponseAPI<bool>
                {
                    Message = "Created",
                    Result = await _I.Insert(entity)
                }); ;
            }
            catch
            {
               // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id,Comment entity)
        {
            try
            {
                entity.Id = id;
               var IsSuccess= await _I.Update(entity);
                return StatusCode(200, new ResponseAPI<string>
                {
                    Message = IsSuccess ? "Success" : "Failed"
                }.Format()) ;
            }
            catch 
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
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
