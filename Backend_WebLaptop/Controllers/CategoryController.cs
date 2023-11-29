﻿using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _i;
        public CategoryController(ICategoryRepository i)
        {
            _i = i;
        }
        //admin
        [HttpGet]
        public async Task<ActionResult> GetList_Category(string? keywords, string? sort, int pageindex = 1, int pagesize = 25)
        {
            return StatusCode(200, new ResponseApi<PagingResult<Category>>
            {
                Message = "Success",
                Result = await _i.GetAll(null,keywords,sort??"name",pageindex,pagesize)
            });
        }
        //admin
        [HttpGet("getbyname")]
        public async Task<ActionResult> FindCategorybyName(string name)
        {
            return StatusCode(200, new ResponseApi<List<Category>>
            {
                Message = "Success",
                Result = await _i.GetAllCategorybyName(name)
            });
        }
        //admin
        [HttpGet("{ParentCategoryId}")]
        public async Task<ActionResult> GetListChildsById(string ParentCategoryId, string? keywords, string? sort, int pageindex=1, int pagesize=25)
        {
            return StatusCode(200, new ResponseApi<PagingResult<Category>>
            {
                Message = "Success",
                Result = await _i.GetAll(ParentCategoryId, keywords, sort ?? "name", pageindex, pagesize)
            });

        }
        //admin
        [HttpGet("getbyId")]
        public async Task<ActionResult> GetbyId(string id)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Category>
                {
                    Message = "Success",
                    Result = await _i.GetbyId(id)
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
           
        }
        //admin
        [HttpPost]
        public async Task<ActionResult> Insert_new(Category entity)
        {
            try
            {
                return StatusCode(201, new ResponseApi<Category>
                {
                    Message = "Created",
                    Result = await _i.Insert(entity)
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        //admin
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Category entity)
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
            catch(Exception ex)
            {
                // Console.WriteLine(e.Message);
                return BadRequest(new ResponseApi<string> { Message=ex.Message,Result= "Failed" });
            }
        }
        //admin
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
        [HttpGet("get-childs-category")]
        public async Task<ActionResult> GetlistChild(string parrentID)
        {
            try
            {
                return StatusCode(200, new ResponseApi<List<Category>>
                {
                    Message = "Success",
                    Result = await _i.GetListChildsById(parrentID)
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi<bool>
                {
                    Message= ex.Message,
                    Result=false
                });
            }
        }
    }
}
