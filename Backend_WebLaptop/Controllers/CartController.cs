using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _I;
        public CartController(ICartRepository i)
        {
            _I = i;
        }
        [HttpGet]
        //sau thêu authentication vào đây
        public async Task<ActionResult> Get(string accountid)
        {
            return StatusCode(200, new ResponseAPI<Cart>
            {
                Message = "Success",
                Result = await _I.GetCart(accountid)
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult> Insert_new(string accountid,CartItem item)
        {
            try
            {
                return await _I.AddtoCart(item, accountid)
                    ? StatusCode(200, new ResponseAPI<string> { Message = "Success" }.Format())
                    : StatusCode(400, new ResponseAPI<string> { Message = "failed" }.Format());
            }
            catch
            {
               // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPost("remove")]
        public async Task<ActionResult> Remove_item(string accountid, CartItem item)
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
        public async Task<ActionResult> DeleteCart(string Userid)
        {
            try
            {
                var IsSuccess = await _I.EmptyCart(Userid);
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
        }
    }
}
