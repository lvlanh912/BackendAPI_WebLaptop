using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewRepository _i;
        public NewsController(INewRepository i)
        {
            _i = i;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get_Post(string id)
        {
            try
            {
                return StatusCode(200, new ResponseApi<News>
                {
                    Message = "Success",
                    Result = await _i.GetbyId(id)
                });
            }
            catch
            {
                return StatusCode(503, new ResponseApi<string> { Message = "Some thing went wrong ", Result = null });
            }

        }

        [HttpGet]
        public async Task<ActionResult> GetList_Post(string? keywords, DateTime? startdate, DateTime? enddate, string? sort, int pageindex = 1, int pagesize = 25)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<News>>
                {
                    Message = "Success",
                    Result = await _i.GetAll(keywords, startdate, enddate, pageindex, pagesize, sort)
                });
            }
            catch
            {
                return StatusCode(503, new ResponseApi<string> { Message = "Some thing went wrong ", Result = null });
            }

        }
        [HttpPost]
        public async Task<ActionResult> Add([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var news = JsonConvert.DeserializeObject<News>(data);
                return StatusCode(201, new ResponseApi<News>
                {
                    Message = "Success",
                    Result = await _i.Insert(new ImageUpload<News>
                    {
                        Data=news,
                        Images=images
                    })
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
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                return await _i.DeletebyId(id) 
                    ? StatusCode(200, new ResponseApi<string> { Message = "Success" }.Format()) 
                    :BadRequest(new ResponseApi<string> { Message = "Failed" }.Format());
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message }.Format());
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                var news = JsonConvert.DeserializeObject<News>(data);
                news!.Id = this.HttpContext.GetRouteValue("id")!.ToString();
                await _i.Update(new ImageUpload<News> { Data = news, Images = images });
                return StatusCode(200, new ResponseApi<string> { Message = "Update Successfull" }.Format());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message }.Format());
            }
        }

    }
}
