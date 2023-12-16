using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Backend_WebLaptop.Model.OnlinePayment.VNPAY;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _i;
        private readonly IPaymentOnlineRepository _paymentOnline;
        public OrderController(IOrderRepository i, IPaymentOnlineRepository paymentOnline)
        {
            _i = i;
            _paymentOnline = paymentOnline;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> Get(string? accountid, int? status,bool? isPaid, string? paymentId,
            int? minPaid, int? maxPaid,DateTime? startdate, DateTime? enddate, string? sort, int pagesize = 25, int pageindex = 1 )
        {
            return StatusCode(200, new ResponseApi<PagingResult<Order>>
            {
                Message = "Success",
                Result = await _i.GetAllOrders(accountid, status, isPaid, paymentId, minPaid, maxPaid, startdate, enddate,sort??"date_desc", pagesize, pageindex)
            });
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("sum-pending")]
        public async Task<ActionResult> GetPendingOrder()
        {
            try
            {
                return StatusCode(200, new ResponseApi<long>
                {
                    Message = "Success",
                    Result = await _i.Get_CountPending()
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false, Message = ex.Message });
            }
            
           
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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


        [HttpPost("checkout")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> CheckOut(Order entity)
        {
            try
            {
                var accountId = HttpContext.User.FindFirst("Id")!.Value;
                entity.AccountId = accountId;
                //gán entity.id=token request
                return StatusCode(200, new ResponseApi<Order>
                {
                    Result = await _i.Checkout(entity),
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
        
        [HttpPost("create-order")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> CreateOrder(Order entity)
        {
            try
            {
                var accountId = HttpContext.User.FindFirst("Id")!.Value;
                IPAddress ipAddress = IPAddress.Parse(Request.HttpContext.Request.Headers["X-Forwarded-For"]);

                /*  string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                  if (string.IsNullOrEmpty(ip))
                  {
                      ip = Current.Request.ServerVariables["REMOTE_ADDR"];
                  }*/
                entity.AccountId = accountId;
                if (entity.PaymentMethod != null)
                {
                    return StatusCode(200, new ResponseApi<string>
                    {
                        Result =await _paymentOnline.CreatePaymentLinkVNPay(await _i.CreateOrder(entity, false),ipAddress.ToString()),
                        Message = "Đặt hàng thành công, đang chờ thanh toán"
                    });
                }
                return StatusCode(201, new ResponseApi<Order>
                {
                    Result = await _i.CreateOrder(entity, false),
                    Message = "Đặt hàng thành công"
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

        [HttpPatch("cancel-order")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> CancelOrder(string orderid)
        {
            try
            {
                var accountId = HttpContext.User.FindFirst("Id")!.Value;
                await _i.CancelOrder(accountId, orderid);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result =true ,
                    Message = "Thành công"
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

        [HttpGet("my-orders")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> GetMyOrder(int? type, int pagesize = 25, int pageindex = 1)
        {
            try
            {
                var accountId = HttpContext.User.FindFirst("Id")!.Value;
                
                return StatusCode(200, new ResponseApi<PagingResult<Order>>
                {
                    Result = await _i.GetMyOrder(accountId, type,pagesize,pageindex),
                    Message = "Thành công"
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
        
        [HttpPost("vnpay-transaction")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> CheckVNPayment(VNPayResponse payload)
        {
            try
            {
                // var accountId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = true,
                    Message = await _paymentOnline.CheckPaymentVNPay(payload)
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
        
        [HttpGet("vnpay-getlink")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> RegetLinkVNPay(string orderId)
        {
            try
            {
                var accountId = HttpContext.User.FindFirst("Id")!.Value;
                IPAddress ipAddress = IPAddress.Parse(Request.HttpContext.Request.Headers["X-Forwarded-For"]);
                return StatusCode(200, new ResponseApi<string>
                {
                    Result = await _paymentOnline.RegetLinkVNPay(orderId,accountId, ipAddress.ToString()),
                    Message = "Thành công"
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
    }
}
