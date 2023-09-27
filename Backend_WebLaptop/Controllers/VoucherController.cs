using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/vouchers")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _I;
        public VoucherController(IVoucherRepository i)
        {
            _I = i;
        }
        [HttpGet]
        public async Task<ActionResult> Getall(string? keywords, int pageindex = 1, int pagesize = 10, bool disable = false)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<PagingResult<Voucher>>
                {
                    Message = "Success",
                    Result = await _I.GetAllVouchers(keywords, pagesize, pageindex, disable)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Getbyid(string id)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<Voucher>
                {
                    Message = "Success",
                    Result = await _I.GetVoucherbyId(id)
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpGet("GetbyCode")]
        public async Task<ActionResult> GetbyCode(string Code)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<Voucher>
                {
                    Message = "Success",
                    Result = await _I.GetVoucherbyCode(Code)
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
                return StatusCode(201, new ResponseAPI<Voucher>
                {
                    Message = "Success",
                    Result = await _I.CreateVoucher(entity)
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
                return StatusCode(200, new ResponseAPI<Voucher>
                {
                    Message = "Updated",
                    Result = await _I.EditVoucher(entity, id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("disable")]
        public async Task<ActionResult> disable(string voucherID)
        {
            try
            {
                return StatusCode(200, new ResponseAPI<string>
                {
                    Message = await _I.DisableVoucher(voucherID) ? "sucess" : "failed"
                });
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> delete(string id)
        {
            try
            {
                var rs = await _I.DeleteVoucher(id);
                if (rs)
                    return StatusCode(200, new ResponseAPI<string> { Message = "success" }.Format());
                return StatusCode(400, new ResponseAPI<string> { Message = "Something is error" }.Format());
            }
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

    }
}
