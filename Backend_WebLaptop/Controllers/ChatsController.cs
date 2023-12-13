using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatRepository _i;
        public ChatsController(IChatRepository i)
        {
            _i = i;
        }
/*        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]*/
        [HttpGet]
        public async Task<ActionResult> GetAllChatList(string? sort,int pageindex=1,int pagesize=20)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Chat>>
                {
                    Message = "Success",
                    Result = await _i.GetAllChat(pageindex,pagesize,sort??"date_desc")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }

        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("my-chat")]
        public async Task<ActionResult> GetMyChat()
        {
            try
            {
                var accounId = HttpContext.User.FindFirst("Id")!.Value;
                return StatusCode(200, new ResponseApi<Chat>
                {
                    Message = "Success",
                    Result = await _i.GetChat(accounId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }

        }
    }
}
