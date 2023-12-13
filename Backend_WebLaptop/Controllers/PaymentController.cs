using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _i;
        public PaymentController(IPaymentRepository i)
        {
            _i = i;
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet]
        public async Task<ActionResult> GetList_Payment()
        {
            return StatusCode(200, new ResponseApi<List<Payment>>
            {
                Message = "Success",
                Result = await _i.GetAll()
            });
        }


        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("list-payments")]
        public async Task<ActionResult> GetPaymentActive()
        {
            try
            {
                return StatusCode(200, new ResponseApi<List<Payment>>
                {
                    Message = "Success",
                    Result = await _i.GetActivePayment()
                });
            }
           
              catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPost]
        public async Task<ActionResult> Insert_new(Payment entity)
        {
            try
            {
                return StatusCode(201, new ResponseApi<Payment>
                {
                    Message = "Created",
                    Result = await _i.Insert(entity)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Payment entity)
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false, Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false, Message = ex.Message });
            }
        }
    }
}
