using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _i;
        public OrderController(IOrderRepository i)
        {
            _i = i;
        }
        [HttpGet]
        //role admin
        public async Task<ActionResult> Get(string? accountid, string? keywords, string? paymentId, int pagesize = 10, int pageindex = 1, int start = 30, int end = 0)
        {
            return StatusCode(200, new ResponseApi<PagingResult<Order>>
            {
                Message = "Success",
                Result = await _i.GetAllOrders(accountid, keywords, paymentId, pagesize, pageindex, start, end)
            });
        }
        //role admin
        [HttpPost("add")]
        public async Task<ActionResult> Insert_new(Order entity)
        {
            try
            {
                var a = await _i.CreateOrder(entity);
                return StatusCode(200, a);
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        //role admin
        public async Task<ActionResult> Remove_item(string id)
        {
            try
            {
                var rs = await _i.DeleteOrder(id);
                return StatusCode(rs ? 204 : 400, new ResponseApi<string> { Message = rs ? "success" : "failed" }.Format());
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(Order entity, string id)
        {
            try
            {
                var rs = await _i.EditOrder(entity, id);
                return StatusCode(rs != entity ? 200 : 401, new ResponseApi<string> { Message = rs != entity ? "success" : "failed" }.Format());
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
