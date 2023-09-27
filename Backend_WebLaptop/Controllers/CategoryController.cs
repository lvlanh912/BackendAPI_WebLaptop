using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _I;
        public CategoryController(ICategoryRepository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> GetList_Category()
        {
            return StatusCode(200, new ResponseAPI<List<Category>>
            {
                Message = "Success",
                Result = await _I.GetAll()
            });
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new(Category entity)
        {
            try
            {
                return StatusCode(201, new ResponseAPI<Category>
                {
                    Message = "Created",
                    Result = await _I.Insert(entity)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Category entity)
        {
            try
            {
                entity.Id = id;
                var IsSuccess = await _I.Update(entity);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var IsSuccess = await _I.DeletebyId(id);
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
