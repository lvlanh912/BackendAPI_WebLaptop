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
            entity.Code = entity.Code!.ToUpper().Replace(" ", String.Empty);
            if (entity.IsValue)
                entity.MaxReduce = entity.Value;
            await Vouchers.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> DisableVoucher(string voucherId)
        {
            var update = Builders<Voucher>.Update.Set(e => e.IsDisable, true);
            var rs= await Vouchers.UpdateOneAsync(e => e.Id == voucherId, update);
            return rs.ModifiedCount > 0;
        }
        public async Task<bool> DeleteVoucher(string voucherId)
        {
            var rs = await Vouchers.FindOneAndDeleteAsync(e => e.Id == voucherId);
            return rs!=null;
        }

        public async Task<Voucher> EditVoucher(Voucher entity, string voucherId)
        {
            var curent = await Vouchers.FindSync(e => e.Id == voucherId).FirstOrDefaultAsync();
            if (curent == null)
                throw new Exception("This voucher does not exits");
            entity.Id = voucherId;
            entity.CreateAt = curent.CreateAt;
            entity.Code = entity.Code!=null?entity.Code.ToUpper().Replace(" ",String.Empty):curent.Code;
            entity.MinApply = (entity.MinApply != null ? entity.MinApply : curent.MinApply);
            entity.Value = entity.Value >= 0 ? entity.Value : curent.Value;
            entity.MaxReduce = entity.MaxReduce <= 0 ? curent.MaxReduce : entity.MaxReduce;
            entity.Quantity = entity.Quantity == null ? curent.Quantity : entity.Quantity;
            entity.StartAt = entity.StartAt == null ? curent.StartAt : entity.StartAt;
            entity.EndAt = entity.EndAt == null ? curent.EndAt : entity.EndAt;
            if (!Validate(entity))
                throw new Exception("Data invalid");
            await Vouchers.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
            return entity;
        }

        public async Task<PagingResult<Voucher>> GetAllVouchers(string? keywords, int PageSize, int pageindex,bool isdisable)
        {
            //filter
            var all =string.IsNullOrWhiteSpace(keywords)? await Vouchers.FindSync(e => true).ToListAsync():
                await Vouchers.FindSync(e => e.Code!.Contains(keywords.Trim().ToUpper())).ToListAsync();
            if (isdisable)
               all= all.FindAll(e => e.IsDisable ==true);
            return new PagingResult<Voucher>
            {
                Items = all,
                PageIndex = pageindex,
                PageSize = PageSize
            };
        }

        public async Task<Voucher> GetVoucherbyCode(string voucherCode)
        {
            return await Vouchers.FindSync(e => e.Code == voucherCode).FirstOrDefaultAsync();
        }

        public async Task<Voucher> GetVoucherbyId(string voucherId)
        {
            return await Vouchers.FindSync(e => e.Id == voucherId).FirstOrDefaultAsync();
        }

        static bool Validate(Voucher entity)
        {
            var list = new List<bool>
            {
                string.IsNullOrWhiteSpace(entity.Code),
                entity.MinApply==null||entity.MinApply<0,
                entity.Value<=0,
                 entity.MaxReduce<=0,
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



        public async Task<bool> IsValidCode(string Code)
        {
            var rs = await Vouchers.FindSync(e => e.Code == Code&&e.IsDisable==false).FirstOrDefaultAsync();
            return rs != null;
        }
    }
}
