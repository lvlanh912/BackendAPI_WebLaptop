using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticRepository _i;
        public StatisticController(IStatisticRepository i)
        {
            _i = i;
        }
        
        //Số lượng sản phẩm còn - hết hàng
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("product-stock")]
        public async Task<ActionResult> GetStockProductData()
        {
            try
            {
                return StatusCode(200, new ResponseApi<object>
                {
                    Result = await _i.GetStockStatistic(),
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        
        //số lượng sản phẩm được thêm trong năm
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("product-add-year")]
        public async Task<ActionResult> GetAddProductYear(int year)
        {
            try
            {
                if(year>DateTime.Now.Year||year<2023)
                    return BadRequest(new ResponseApi<string> { Message = "inValid Year" });
                return StatusCode(200, new ResponseApi<List<long>>
                {
                    Result = await _i.GetProductAddStatistic(year),
                    Message = "Thành công"
                }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }

        //Top sản phẩm bán chạy trong khoảng thời gian
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("top-product-sold")]
        public async Task<ActionResult> GetTopSold(DateTime starttime,DateTime? endtime, int value = 10)
        {
            try
            {
                
                return StatusCode(200, new ResponseApi<List<Product>>
                {
                    Result = await _i.GetTop(value,starttime,endtime,1),
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        
        //Top sản phẩm có nhiều lượt xem nhất
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("top-product-view")]
        public async Task<ActionResult> GetTopView(DateTime starttime, DateTime? endtime, int value = 10)
        {
            try
            {

                return StatusCode(200, new ResponseApi<List<Product>>
                {
                    Result = await _i.GetTop(value, starttime, endtime, 2),
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        
        //Top sản phẩm còn tồn kho nhiều nhất
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("top-product-stock")]
        public async Task<ActionResult> GetTopStock(DateTime starttime, DateTime? endtime, int value = 10)
        {
            try
            {

                return StatusCode(200, new ResponseApi<List<Product>>
                {
                    Result = await _i.GetTop(value, starttime, endtime, 3),
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }

        //Tổng quan đơn hàng
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("order-overview")]
        public async Task<ActionResult> GetOrderStatistic( )
        {
            try
            {

                return StatusCode(200, new ResponseApi<object>
                {
                    Result = await _i.GetOrderStatistic(),
                    Message = "Thành công"
                }); ;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        [HttpGet("order-per-month")]
        public async Task<ActionResult> GetorderAddperMonth(DateTime startDate, DateTime endDate)
        {
            try
            {

                return StatusCode(200, new ResponseApi<List<long>>
                {
                    Result = await _i.GetOrderPerMonth(startDate,endDate),
                    Message = "Thành công"
                }); ;
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<string> { Message = ex.Message });
            }
        }
    }
}
