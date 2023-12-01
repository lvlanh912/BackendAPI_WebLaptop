using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _i;
        public CommentController(ICommentRepository i)
        {
            _i = i;
        }
        //admin
        [HttpGet]
        public async Task<ActionResult> Get_Commnents_byProductId(string? accountid,string? productId,string? keywords,string? sort,int pageindex=1,int pagesize=25)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Comment>>
                {
                    Message = "Success",
                    Result = await _i.GetAll(accountid, productId, keywords,sort??"date",pageindex, pagesize)
                });
            }
            catch
            {
                return BadRequest(new ResponseApi<bool>
                {
                    Result = false
                }) ;
            }
        }
        //admin
        [HttpGet("sum-create")]
        public async Task<ActionResult> GetTotalCreate(DateTime? start, DateTime? end)
        {
            try
            {
                return StatusCode(200, new ResponseApi<long>
                {
                    Result = await _i.GetTotalCreatebyTime(start ?? DateTime.Today, end ?? DateTime.Now),
                    Message = "Success"

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false, Message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new(Comment entity)
        {
            try
            {
                string productId = this.RouteData.Values["ProductId"]!.ToString()!;
                entity.ProductId = productId;
                return StatusCode(201, new ResponseApi<bool>
                {
                    Message = "Created",
                    Result = await _i.Insert(entity)
                }); ;
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Comment entity)
        {
            try
            {
                entity.Id = id;
                var isSuccess = await _i.Update(entity);
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = isSuccess ? "Success" : "Failed"
                }.Format());
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
                var isSuccess = await _i.DeletebyId(id);
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = isSuccess ? "Success" : "Failed"
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
