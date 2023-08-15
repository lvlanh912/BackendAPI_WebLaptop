using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IMongoCollection<Ward>? _Wards;
        private readonly IMongoCollection<Province>? _Provinces;
        private readonly IMongoCollection<District>? _Districts;

        public AddressRepository(IDatabase_Service database_Service)
        {
            _Wards = database_Service.Get_Ward_Collection();
            _Provinces = database_Service.Get_Provinces_Collection();
            _Districts=database_Service.Get_District_Collection();
        }
        public async Task<List<Province>> GetAllProvince()
        {
          var rs= await _Provinces.FindAsync(e => true);
            return await rs.ToListAsync();
        }
        public async Task<List<District>> GetListDistrict(int ProvinceCode)
        {
            var rs= await _Districts.FindAsync(e=>e.ProvinceCode== ProvinceCode);
            return await rs.ToListAsync();
        }

        public async Task<List<Ward>> GetListWard(int DistrictCode)
        {
            var rs = await _Wards.FindAsync(e => e.DistrictCode == DistrictCode);
            return await rs.ToListAsync();
        }
        public async Task<Ward> GetWardbyId(string wardId)
        {
            return await _Wards.FindSync(e => e.Id == wardId).FirstOrDefaultAsync();
        }
    }
}
