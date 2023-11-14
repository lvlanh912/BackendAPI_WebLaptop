using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class ShippingAddressRepository : IShippingAddressRepository
    {
        private readonly IMongoCollection<ShippingAddress>? _shippingAddresses;
        private readonly IAddressRepository? _address;

        public ShippingAddressRepository(IDatabaseService databaseService, IAddressRepository i)
        {
            this._address = i;
            _shippingAddresses = databaseService.Get_ShippingAddress_Collection();
        }

        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _shippingAddresses.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount > 0;
        }

        public async Task<List<ShippingAddress>> GetAll(string accountId)
        {
            var rs = await _shippingAddresses.FindAsync(e => e.AccountId == accountId);
            return await rs.ToListAsync();
        }

        public async Task<ShippingAddress> GetbyId(string id)
        {
            return await _shippingAddresses.FindSync(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ShippingAddress> Insert(ShippingAddress entity)
        {
            if (!await ValidateDataAsync(entity))
                throw new Exception("Invalid data");

            await _shippingAddresses!.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> Update(ShippingAddress entity)
        {
            if (entity.Id == null)
                return false;
            var current = await GetbyId(entity.Id);
            if (current != null)
            {
                entity.WardId ??= current.WardId;
                entity.AccountId ??= current.AccountId;
                entity.Address ??= current.Address;
                entity.Fullname ??= current.Fullname;
                //nếu không tồn tại Wardid thì không thực hiện update
                if (await ValidateDataAsync(entity))
                {
                    var rs = await _shippingAddresses.ReplaceOneAsync(e => e.Id == entity.Id, entity);
                    return rs.ModifiedCount > 0;
                }
                throw new Exception("Invalid data");
            }
            return false;
        }
        private async Task<bool> ValidateDataAsync(ShippingAddress entity)
        {
            if (string.IsNullOrEmpty(entity.WardId) || string.IsNullOrEmpty(entity.Fullname) ||
                string.IsNullOrEmpty(entity.Address) || !Isphone(entity.Phone))
                return false;
            var ward = await _address!.GetWardbyId(entity.WardId);
            return ward != null;
        }
        static bool Isphone(Int32 phone) => phone < 999999999 && phone > 300000000;//đầu 03 đến 09
    }
}
