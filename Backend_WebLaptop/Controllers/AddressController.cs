using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _i;
        public AddressController(IAddressRepository i)
        {
            _i = i;
        }
        [HttpGet("provinces")]
        public async Task<ActionResult> GetList_Provinces()
        {
            return StatusCode(200, new ResponseApi<List<Province>>
            {
                Message = "Success",
                Result = await _i.GetAllProvince()
            });
        }
        [HttpGet("districts")]
        public async Task<ActionResult> GetList_District(int provinceCode)
        {
            return StatusCode(200, new ResponseApi<List<District>>
            {
                Message = "Success",
                Result = await _i.GetListDistrict(provinceCode)
            });
        }
        [HttpGet("wards")]
        public async Task<ActionResult> GetList_Ward(int districtCode)
        {
            return StatusCode(200, new ResponseApi<List<Ward>>
            {
                Message = "Success",
                Result = await _i.GetListWard(districtCode)
            });
        }
        [HttpGet("ward{id}")]
        public async Task<ActionResult> Get_Ward(string id)
        {
            return StatusCode(200, new ResponseApi<Ward>
            {
                Message = "Success",
                Result = await _i.GetWardbyId(id)
            });
        }
        [HttpGet("{wardId}")]
        public async Task<ActionResult> GetFulladdress(string wardId)
        {
            try
            {
                return StatusCode(200, new ResponseApi<string>
                {
                    Message = "Success",
                    Result = await _i.GetAddress(wardId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi<bool> { Message = ex.Message, Result = false });
            }

        }
    }
}
