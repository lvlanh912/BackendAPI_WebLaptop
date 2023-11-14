using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _i;
        public CartController(ICartRepository i)
        {
            _i = i;
        }
        [HttpGet]
        //sau thêu authentication vào đây
        public async Task<ActionResult> Get(string accountid)
        {
            return StatusCode(200, new ResponseApi<Cart>
            {
                Message = "Success",
                Result = await _i.GetCart(accountid)
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult> Insert_new(string accountid, CartItem item)
        {
            try
            {
                return await _i.AddtoCart(item, accountid)
                    ? StatusCode(200, new ResponseApi<string> { Message = "Success" }.Format())
                    : StatusCode(400, new ResponseApi<string> { Message = "failed" }.Format());
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
                return await _i.DeleteItem(item, accountid)
                    ? StatusCode(200, new ResponseApi<string> { Message = "Success" }.Format())
                    : StatusCode(400, new ResponseApi<string> { Message = "failed" }.Format());
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCart(string userid)
        {
            try
            {
                var isSuccess = await _i.EmptyCart(userid);
                return StatusCode(200, new ResponseApi<string>
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
