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
        [ServiceFilter(typeof(SessionAuthor))]
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
        [ServiceFilter(typeof(SessionAuthor))]
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
       
        [HttpPatch]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> UpdateOne(CartItem cartItem)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                await _i.UpdateCart(cartItem, accounId,false);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = true,
                    Message="Updated"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [HttpPatch("delete-item")]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> DeleteOneItem(CartItem cartItem)
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                await _i.UpdateCart(cartItem, accounId, true);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = true,
                    Message = "Updated"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task<ActionResult> EmptyCart()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                await _i.EmptyCart(accounId);
                return StatusCode(200, new ResponseApi<bool>
                {
                    Result = true,
                    Message = "Success"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }
        }
    }
}
