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
        //role admin
        public async Task<ActionResult> Get(string? accountid, string? keywords, string? paymentID, int pagesize = 10, int pageindex = 1, int start = 30, int end = 0)
        {
            return StatusCode(200, new ResponseAPI<PagingResult<Order>>
            {
                Message = "Success",
                Result = await _I.GetAllOrders(accountid, keywords, paymentID, pagesize, pageindex, start, end)
            });
        }
        //role admin
        [HttpPost("add")]
        public async Task<ActionResult> Insert_new(Order entity)
        {
            try
            {
                var a = await _I.CreateOrder(entity);
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
                var rs = await _I.DeleteOrder(id);
                return StatusCode(rs ? 204 : 400, new ResponseAPI<string> { Message = rs ? "success" : "failed" }.Format());
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> edit(Order entity, string id)
        {
            try
            {
                var rs = await _I.EditOrder(entity, id);
                return StatusCode(rs != entity ? 200 : 401, new ResponseAPI<string> { Message = rs != entity ? "success" : "failed" }.Format());
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
