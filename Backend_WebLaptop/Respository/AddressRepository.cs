using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IMongoCollection<Ward>? _wards;
        private readonly IMongoCollection<Province>? _provinces;
        private readonly IMongoCollection<District>? _districts;

        public AddressRepository(IDatabaseService databaseService)
        {
            _wards = databaseService.Get_Ward_Collection();
            _provinces = databaseService.Get_Provinces_Collection();
            _districts = databaseService.Get_District_Collection();
        }

        public async Task<string> GetAddress(string wardId)
        {
            var xa = await _wards.FindSync(e => e.Id == wardId).FirstOrDefaultAsync() ?? throw new Exception("not availid wardId");
            var huyen = await _districts.FindSync(e => e.Code == xa.DistrictCode).FirstOrDefaultAsync();
            var tinh = await _provinces.FindSync(e => e.Code == huyen.ProvinceCode).FirstOrDefaultAsync();
            return xa.Name + '-' + huyen.Name + '-' + tinh.Name;
        }

        public async Task<List<Province>> GetAllProvince()
        {
            var rs = await _provinces.FindAsync(e => true);
            return await rs.ToListAsync();
        }
        public async Task<List<District>> GetListDistrict(int provinceCode)
        {
            var rs = await _districts.FindAsync(e => e.ProvinceCode == provinceCode);
            return await rs.ToListAsync();
        }

        public async Task<List<Ward>> GetListWard(int districtCode)
        {
            var rs = await _wards.FindAsync(e => e.DistrictCode == districtCode);
            return await rs.ToListAsync();
        }
        public async Task<Ward> GetWardbyId(string wardId)
        {
            return await _wards.FindSync(e => e.Id == wardId).FirstOrDefaultAsync();
        }
    }
}
