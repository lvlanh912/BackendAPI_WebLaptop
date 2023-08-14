using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IAddressRespository
    {
        Task<List<Province>> GetAllProvince();
        Task<List<District>> GetListDistrict( int ProvinceCode);
        Task<List<Ward>> GetListWard(int DistrictCode);
        Task<Ward> GetWardbyId(string wardId);
    }
}
