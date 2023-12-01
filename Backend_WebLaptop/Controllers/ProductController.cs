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
        public async Task<ActionResult> Get_Products( string? keywords, string? brand,bool?stock, string? category, int? min, int? max, string? sort, int pageindex=1, int Pagesize=25)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Product>>
                {
                    Message = "Success",
                    Result = await _i.GetAll( keywords, stock,  brand,category,  min,  max, sort??"date",  pageindex, Pagesize )
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<string>
                {
                    Message = ex.Message
                }); 
            }
        }

        [HttpGet("sum-out-stock")]
        public async Task<ActionResult> GetTotalOutStock()
        {
            try
            {
                return StatusCode(200, new ResponseApi<long>
                {
                    Result = await _i.GetTotalOutStock(),
                    Message = "Success"

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Result = false, Message = ex.Message });
            }
        }

            [HttpGet("search")]
        public async Task<ActionResult> Get_Products_keyword(string keywords)
        {
            try
            {
                return StatusCode(200, new ResponseApi<List<Product>>
                {
                    Message = "Success",
                    Result = await _i.GetbyKeyword(keywords)
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
        [HttpGet("{id}")]
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
        [HttpPost("view")]
        public async Task<ActionResult> AddView(string id)
        {
            try
            {
                await  _i.InsertView(id);
                return StatusCode(201, new ResponseApi<bool>
                {
                    Result = true
                });
            }
            catch
            {
                return BadRequest( new ResponseApi<bool>
                {
                    Result = false
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new([FromForm] string data, List<IFormFile>? images)
        {
            try
            {
                return StatusCode(201, new ResponseApi<Product>
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
                return BadRequest(new ResponseApi<string>
                {
                    Message = ex.Message
                });
                
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromForm]string data, List<IFormFile>? images)
        {
            try
            {
                var product = JsonConvert.DeserializeObject<Product>(data);
                product!.Id = this.HttpContext.GetRouteValue("id")!.ToString();
                return StatusCode(200, new ResponseApi<Product>
                {
                    Message = "Success",
                    Result = await _i.Update(new ImageUpload<Product> { Data = product, Images = images })
                }) ;
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<string>
                {
                    Message=ex.Message,
                    Result="Failed"
                });
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