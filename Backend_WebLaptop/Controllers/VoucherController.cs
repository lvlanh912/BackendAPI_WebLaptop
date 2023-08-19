using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/voucher")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _I;
        public VoucherController(IVoucherRepository i)
        {
            _I = i;
        }
      
        [HttpPost("add")]
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
            catch
            {
               // Console.WriteLine(e.Message);
                return BadRequest();
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
            catch
            {
                // Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
       
    }
}
