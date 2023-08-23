using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _I;
        public OrderController(IOrderRepository i)
        {
            _I = i;
        }
        [HttpGet]
        //sau thêu authentication vào đây
        public async Task<ActionResult> Get(string? accountid,string? keywords, string? paymentID, int pagesize=10,int pageindex=1,int start=30,int end=0)
        {
            return StatusCode(200, new ResponseAPI<PagingResult<Order>>
            {
                Message = "Success",
                Result = await _I.GetAllOrders(accountid,keywords, paymentID, pagesize,pageindex,start,end)
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult> Insert_new(Order entity)
        {
            try
            {
                var a = await _I.CreateOrder(entity);
                return StatusCode(200, a);
            }
            catch(Exception ex)
            {
               // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }
      /*  [HttpPost("remove")]
        public async Task<ActionResult> Remove_item(string accountid, OrderItem item)
        {
            try
            {
                return await _I.DeleteItem(item, accountid)
                    ? StatusCode(200, new ResponseAPI<string> { Message = "Success" }.Format())
                    : StatusCode(400, new ResponseAPI<string> { Message = "failed" }.Format());
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrder(string Userid)
        {
            try
            {
                var IsSuccess = await _I.EmptyOrder(Userid);
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
        }*/
    }
}
