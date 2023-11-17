using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/vouchers")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _i;
        public VoucherController(IVoucherRepository i)
        {
            _i = i;
        }
        [HttpGet]
        public async Task<ActionResult> Getall(string? keywords,DateTime? createTimeStart, DateTime? createTimeEnd, bool? active, string? sort, int pageindex = 1, int pagesize = 10)
        {
            try
            {
                return StatusCode(200, new ResponseApi<PagingResult<Voucher>>
                {
                    Message = "Success",
                    Result = await _i.GetAllVouchers(keywords, createTimeStart, createTimeEnd, active, pagesize, pageindex,sort??"create" )
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ResponseApi<string> { Message=ex.Message});
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Getbyid(string id)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Voucher>
                {
                    Message = "Success",
                    Result = await _i.GetVoucherbyId(id)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpGet("GetbyCode")]
        public async Task<ActionResult> GetbyCode(string code)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Voucher>
                {
                    Message = "Success",
                    Result = await _i.GetVoucherbyCode(code)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Insert_new(Voucher entity)
        {
            try
            {
                return StatusCode(201, new ResponseApi<Voucher>
                {
                    Message = "Success",
                    Result = await _i.CreateVoucher(entity)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Voucher entity)
        {
            try
            {
                return StatusCode(200, new ResponseApi<Voucher>
                {
                    Message = "Updated",
                    Result = await _i.EditVoucher(entity, id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("disable")]
        public async Task<ActionResult> Disable(string voucherId)
        {
            try
            {
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = await _i.DisableVoucher(voucherId) ? "sucess" : "failed"
                });
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
                var rs = await _i.DeleteVoucher(id);
                if (rs)
                    return StatusCode(200, new ResponseApi<string> { Message = "success" }.Format());
                return StatusCode(400, new ResponseApi<string> { Message = "Something is error" }.Format());
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

    }
}
