using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users/")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressRepository _i;
        public ShippingAddressController(IShippingAddressRepository i)
        {
            _i = i;
        }
        [HttpGet("{userId}/shipping-address")]
        public async Task<ActionResult> GetList_ShippingAddress(string userId)
        {
            return StatusCode(200, new ResponseApi<List<ShippingAddress>>
            {
                Message = "Success",
                Result = await _i.GetAll(userId)
            });
        }
        [HttpPost("{userId}/shipping-address")]
        public async Task<ActionResult> Insert_new(ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId = this.RouteData.Values["userID"]!.ToString();
                return StatusCode(201, new ResponseApi<ShippingAddress>
                {
                    Message = "Created",
                    Result = await _i.Insert(shippingAddress)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{userId}/shipping-address/{id}")]
        public async Task<ActionResult> Update(string id, ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId = this.RouteData.Values["userID"]!.ToString();
                shippingAddress.Id = id;
                var isSuccess = await _i.Update(shippingAddress);
                return StatusCode(200, new ResponseApi<ShippingAddress>
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
        [HttpDelete("{userId}/shipping-address/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var isSuccess = await _i.DeletebyId(id);
                return StatusCode(200, new ResponseApi<ShippingAddress>
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
