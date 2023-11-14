using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _i;
        public ProductController(IProductRepository i)
        {
            _i = i;
        }
        [HttpGet]
        public async Task<ActionResult> Get_Products(int? pageindex, int? size, string? keywords, string? brand,
            string? category, int? min, int? max)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Product>>
                {
                    Message = "Success",
                    Result = await _i.GetAll(new ProductFilter
                    {
                        Brand = brand,
                        Keywords = keywords,
                        Category = category,
                        MinPrice = min,
                        MaxPrice = max
                    }, pageindex ?? 1, size ?? 10)
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("id")]
        public async Task<ActionResult> GetById(string id)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Product>
                {
                    Message = "Success",
                    Result = await _i.GetbyId(id)
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                return StatusCode(201, new ResponseApi<bool>
                {
                    Message = "Created",
                    Result = await _i.Insert(new ImageUpload<Product>
                    {
                        Data = JsonConvert.DeserializeObject<Product>(data),
                        Images = images
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Product entity)
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
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
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
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPost("giam-sp")]
        public async Task<ActionResult> Descrease(List<OrderItem> entity)
        {
            try
            {
                var rs = await _i.DecreaseQuantity(entity);
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = rs ? "Success" : "Failed"
                }.Format());
            }
            catch (Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}