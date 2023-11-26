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
        public async Task<ActionResult> Get(string? accountid, int? status,bool? isPaid, string? paymentId,
            int? minPaid, int? maxPaid,DateTime? startdate, DateTime? enddate, string? sort, int pagesize = 25, int pageindex = 1 )
        {
            return StatusCode(200, new ResponseApi<PagingResult<Order>>
            {
                Message = "Success",
                Result = await _i.GetAllOrders(accountid, status, isPaid, paymentId, minPaid, maxPaid, startdate, enddate,sort??"date", pagesize, pageindex)
            });
        }
        //role admin or role user
        [HttpPost("create")]
        public async Task<ActionResult> Insert_new(Order entity)
        {
            try
            {
                //gán entity.id=token request
                return StatusCode(201, new ResponseApi<Order>
                {
                    //mặc định admin (chưa sửa)
                    Result =await _i.CreateOrder(entity,true),
                    Message = "Đặt hàng thành công"

                }) ;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string>
                {
                    Message = ex.Message
                });
            }
        }
        //role admin or role user
        [HttpPost("checkin")]
        public async Task<ActionResult> Checkin(Order entity)
        {
            try
            {
                //gán entity.id=token request
                return StatusCode(201, new ResponseApi<string>
                {
                    Result ="",
                    Message = "Success"

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string>
                {
                    Message = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        //role admin
        public async Task<ActionResult> DeleteOrder(string id)
        {
            try
            {
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = await _i.DeleteOrder(id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool>
                {
                    Result = false,
                    Message = ex.Message
                }) ;
            }
        }
        [HttpPut("{id}")]
        //role Admin
        public async Task<ActionResult> Edit(string id,int ? status,ShippingAddress? shippingAddress,bool? ispaid )
        {
            try
            {
                var rs = await _i.EditOrder(id,status, shippingAddress,ispaid);
                return StatusCode(200,new ResponseApi<bool> { Result=true});
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(new ResponseApi<bool> { Result = false,Message=ex.Message });
            }
        }

    }
}
