using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(200, new ResponseApi<Cart>
                {
                    Message = "Success",
                    Result = await _i.GetCart(accounId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [HttpPost("add")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> Insert_new( CartItem item)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(201, new ResponseApi<bool> {
                    Message = "Success",Result= await _i.AddtoCart(item, accounId) 
                }.Format());
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message=ex.Message,Result=false});
            }
        }
        [HttpPost("remove")]
        public async Task<ActionResult> Remove_item(string accountid, CartItem item)
        {
            try
            {
                return await _i.DeleteItem(item, accountid)
                    ? StatusCode(200, new ResponseApi<string> { Message = "Success" }.Format())
                    : StatusCode(400, new ResponseApi<string> { Message = "Failed" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
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
       /* [HttpPatch]
        public async Task<ActionResult> UpdateOne(CartItem cartItem)
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
        }*/
    }
}
