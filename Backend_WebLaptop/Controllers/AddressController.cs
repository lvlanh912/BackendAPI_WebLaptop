using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_WebLaptop.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _I;
        public AddressController(IAddressRepository i)
        {
            _I = i;
        }
        [HttpGet("provinces")]
        public async Task<ActionResult> GetList_Provinces()
        {
            return StatusCode(200, new ResponseAPI<List<Province>>
            {
                Message = "Success",
                Result = await _I.GetAllProvince()
            });
        }
        [HttpGet("districts{ProvinceCode}")]
        public async Task<ActionResult> GetList_District(int ProvinceCode)
        {
            return StatusCode(200, new ResponseAPI<List<District>>
            {
                Message = "Success",
                Result = await _I.GetListDistrict(ProvinceCode)
            });
        }
        [HttpGet("wards{DistrictCode}")]
        public async Task<ActionResult> GetList_Ward(int DistrictCode)
        {
            return StatusCode(200, new ResponseAPI<List<Ward>>
            {
                Message = "Success",
                Result = await _I.GetListWard(DistrictCode)
            });
        }
        [HttpGet("ward{id}")]
        public async Task<ActionResult> Get_Ward(string id)
        {
            return StatusCode(200, new ResponseAPI<Ward>
            {
                Message = "Success",
                Result =await _I.GetWardbyId(id)
            });
        }
    }
}
