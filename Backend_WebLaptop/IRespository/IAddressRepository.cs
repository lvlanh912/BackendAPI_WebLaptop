using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IAddressRepository
    {
        Task<List<Province>> GetAllProvince();
        Task<List<District>> GetListDistrict(int provinceCode);
        Task<List<Ward>> GetListWard(int districtCode);
        Task<Ward> GetWardbyId(string wardId);
        Task<string> GetAddress(string wardId);
    }
}
