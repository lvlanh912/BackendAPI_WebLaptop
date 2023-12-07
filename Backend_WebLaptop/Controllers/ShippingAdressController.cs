using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users/shipping-address")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressRepository _i;
        public ShippingAddressController(IShippingAddressRepository i)
        {
            _i = i;
        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet]
        public async Task<ActionResult> GetList_ShippingAddress()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(200, new ResponseApi<List<ShippingAddress>>
                {
                    Message = "Success",
                    Result = await _i.GetAll(accounId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }

        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPost]
        public async Task<ActionResult> Insert_new(ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId= HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(201, new ResponseApi<ShippingAddress>
                {
                    Message = "Created",
                    Result = await _i.Insert(shippingAddress)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId = HttpContext.User.FindFirst("Id")!.Value;
                shippingAddress.Id = id;
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = await _i.Update(shippingAddress),
                    Message="success"
            }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var accountId= HttpContext.User.FindFirst("Id")!.Value;
                 await _i.DeletebyId(id, accountId);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = true,
                    Message = "deleted"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
    }
}
