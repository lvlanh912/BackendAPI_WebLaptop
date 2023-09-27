using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/users/")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressRepository _I;
        public ShippingAddressController(IShippingAddressRepository i)
        {
            _I = i;
        }
        [HttpGet("{userID}/shipping-address")]
        public async Task<ActionResult> GetList_ShippingAddress(string userID)
        {
            return StatusCode(200, new ResponseAPI<List<ShippingAddress>>
            {
                Message = "Success",
                Result = await _I.GetAll(userID)
            });
        }
        [HttpPost("{userID}/shipping-address")]
        public async Task<ActionResult> Insert_new(ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId = this.RouteData.Values["userID"]!.ToString();
                return StatusCode(201, new ResponseAPI<ShippingAddress>
                {
                    Message = "Created",
                    Result = await _I.Insert(shippingAddress)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{userID}/shipping-address/{id}")]
        public async Task<ActionResult> Update(string id, ShippingAddress shippingAddress)
        {
            try
            {
                shippingAddress.AccountId = this.RouteData.Values["userID"]!.ToString();
                shippingAddress.Id = id;
                var IsSuccess = await _I.Update(shippingAddress);
                return StatusCode(200, new ResponseAPI<ShippingAddress>
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
        [HttpDelete("{userID}/shipping-address/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var IsSuccess = await _I.DeletebyId(id);
                return StatusCode(200, new ResponseAPI<ShippingAddress>
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
