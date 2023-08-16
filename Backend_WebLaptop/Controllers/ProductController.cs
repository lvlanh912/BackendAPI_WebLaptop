﻿using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _I;
        public ProductController(IProductRepository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> Get_Products(int? pageindex, int? size,string? keywords,string? Brand,
            string? Category,int? min,int? max)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<PagingResult<Product>>
                {
                    Message = "Success", 
                    Result = await _I.GetAll(new ProductFilter { 
                        Brand=Brand,
                        Keywords=keywords,
                        Category=Category,
                        Min_price=min,
                        Max_price=max
                    }, pageindex??1, size??10)
                }); 
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Insert_new([FromForm] ImageUpload image)
        {
            try
            {
               
                var a = image;
                /* return StatusCode(201, new ResponseAPI<bool>
                 {
                     Message = "Created",
                     Result = await _I.Insert(entity)
                 });*/
                return Ok();
            }
            catch
            {
               // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id,Product entity)
        {
            try
            {
                entity.Id = id;
               var IsSuccess= await _I.Update(entity);
                return StatusCode(200, new ResponseAPI<string>
                {
                    Message = IsSuccess ? "Success" : "Failed"
                }.Format()) ;
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