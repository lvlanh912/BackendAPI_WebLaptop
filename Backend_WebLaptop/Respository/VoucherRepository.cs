using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly IMongoCollection<Voucher> Vouchers;
        public VoucherRepository(IDatabase_Service database_Service)
        {
            Vouchers = database_Service.Get_Vouchers_Collection();
        }
        public async Task<Voucher> CreateVoucher(Voucher entity)
        {
            entity.CreateAt = DateTime.Now;
            if (!Validate(entity))
                throw new Exception("Invalid data");
            await Vouchers.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteVoucher(string voucherId)
        {
            var update = Builders<Voucher>.Update.Set(e => e.IsDelete, true);
            var rs= await Vouchers.UpdateOneAsync(e => e.Id == voucherId, update);
            return rs.ModifiedCount > 0;
        }

        public async Task<Voucher> EditVoucher(Voucher entity, string voucherId)
        {
            //var update = Builders<Voucher>.Update.Set(e => e.IsDelete, true);
            //khởi tạo các thay đổi
            
            var builder_update = Builders<Voucher>.Update.Set(e => e.Code, entity.Code); 
            /* builder_update.Set(e => e.IsDelete, entity.IsDelete);
             builder_update.Set(e=>e.IsValue, entity.IsValue);*/
            if (entity.Quantity!=null)
                builder_update.Set(e=>e.Quantity,entity.Quantity);
           // var combinedUpdate = builder_update.Combine();
            var rs = await Vouchers.UpdateOneAsync(e => e.Id == voucherId, builder_update);
            return rs.ModifiedCount > 0 ? entity : throw new Exception("Not update");
        }

        public Task<PagingResult<Voucher>> GetAllVouchers(int PageSize = 10, int pageindex = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Voucher> GetVoucherbyCode(string voucherCode)
        {
            throw new NotImplementedException();
        }

        public Task<Voucher> GetVoucherbyId(string voucherId)
        {
            throw new NotImplementedException();
        }

        static bool Validate(Voucher entity)
        {
            var list = new List<bool>
            {
                string.IsNullOrWhiteSpace(entity.Code),
                entity.MinApply==null||entity.MinApply<0,
                entity.Value==null||entity.Value<=0,
                entity.MaxReduce==null|| entity.MaxReduce<=0,
                entity.Quantity==null||entity.Quantity<=0,
                entity.StartAt==null,
                entity.EndAt==null,
                entity.CreateAt==null,
            };
            foreach(var item in list)
            {
                if (item == true)
                    return false;
            }
            return true;
        }
    }
}
