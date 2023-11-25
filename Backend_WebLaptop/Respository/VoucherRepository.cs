using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly IMongoCollection<Voucher> _vouchers;
        public VoucherRepository(IDatabaseService databaseService)
        {
            _vouchers = databaseService.Get_Vouchers_Collection();
        }

        public async Task<Voucher> CreateVoucher(Voucher entity)
        {
            entity.CreateAt = DateTime.Now;
            if (entity.IsValue)
                entity.MaxReduce = entity.Value;
            if (!Validate(entity))
                throw new Exception("Invalid data");
            entity.Code = entity.Code!.ToUpper().Replace(" ", String.Empty);
            if (await GetVoucherbyCode(entity.Code) !=null)
                throw new Exception("This Voucher is already exits");
            //Nếu voucher giảm giá theo số tiền số tiền giảm sẽ bằng số tiền giảm tối đa
            if (entity.IsValue)
                entity.MaxReduce = entity.Value;
            await _vouchers.InsertOneAsync(entity);
            return entity;
        }

        public async Task<PagingResult<Voucher>> GetAllVouchers(string? keywords, DateTime? createTimeStart, DateTime? createTimeEnd, bool? active, int pageSize, int pageindex, string sort)
        {
            //filter
            var result = string.IsNullOrWhiteSpace(keywords) ? await _vouchers.FindSync(e => true).ToListAsync() :
                await _vouchers.FindSync(e => e.Code!.Contains(keywords.Trim().ToUpper())).ToListAsync();
            if (active != null)
                result = result.FindAll(e => e.Active == active);
            if (createTimeStart != null)
                result = result.FindAll(e => e.CreateAt >= createTimeStart);
            if (createTimeEnd != null)
                result = result.FindAll(e => e.CreateAt <= createTimeEnd);
            //sort
            result = sort switch
            {
                "startdate_desc" => result.OrderByDescending(e => e.StartAt).ToList(),
                "startdate" => result.OrderBy(e => e.StartAt).ToList(),
                "enddate_desc" => result.OrderByDescending(e => e.EndAt).ToList(),
                "enddate" => result.OrderBy(e => e.EndAt).ToList(),
                "create_desc" => result.OrderByDescending(e => e.CreateAt).ToList(),
                _ => result.OrderBy(e => e.CreateAt).ToList(),
            };
            return new PagingResult<Voucher>
            {
                Items = result.Skip((pageindex-1)*pageSize).Take(pageSize),
                PageIndex = pageindex,
                PageSize = pageSize,
                TotalCount=result.Count
            };
        }

        public async Task<bool> DisableVoucher(string voucherId)
        {
            var update = Builders<Voucher>.Update.Set(e => e.Active, true);
            var rs = await _vouchers.UpdateOneAsync(e => e.Id == voucherId, update);
            return rs.ModifiedCount > 0;
        }
        public async Task<bool> DeleteVoucher(string voucherId)
        {
            var rs = await _vouchers.FindOneAndDeleteAsync(e => e.Id == voucherId);
            return rs != null;
        }

        public async Task<Voucher> EditVoucher(Voucher entity, string voucherId)
        {
            var curent = await _vouchers.FindSync(e => e.Id == voucherId).FirstOrDefaultAsync() ?? throw new Exception("This voucher does not exist");
            entity.Id = voucherId;
            entity.CreateAt = curent.CreateAt;
            entity.Code = entity.Code != null ? entity.Code.ToUpper().Replace(" ", String.Empty) : curent.Code;
            entity.MinApply = (entity.MinApply != null ? entity.MinApply : curent.MinApply);
            entity.Value = entity.Value >= 0 ? entity.Value : curent.Value;
            entity.MaxReduce = entity.MaxReduce <= 0 ? curent.MaxReduce : entity.MaxReduce;
            entity.Quantity = entity.Quantity == null ? curent.Quantity : entity.Quantity;
            entity.StartAt = entity.StartAt == null ? curent.StartAt : entity.StartAt;
            entity.EndAt = entity.EndAt == null ? curent.EndAt : entity.EndAt;
            if (!Validate(entity))
                throw new Exception("Data invalid");
            await _vouchers.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
            return entity;
        }

       

        public async Task<Voucher> GetVoucherbyCode(string voucherCode)
        {
            return await _vouchers.FindSync(e => e.Code == voucherCode).FirstOrDefaultAsync();
        }

        public async Task<Voucher> GetVoucherbyId(string voucherId)
        {
            return await _vouchers.FindSync(e => e.Id == voucherId).FirstOrDefaultAsync();
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
            foreach (var item in list)
            {
                if (item == true)
                    return false;
            }
            return true;
        }



        public bool IsValidCode(Voucher entity)
        {
            if (entity.Quantity == 0)
                return false;
            if (entity.StartAt > DateTime.Now)
                return false;
            if (!entity.Active)
                return false;
            if (entity.EndAt < DateTime.Now)
                return false;
            return true;
        }

        public async Task Decrease(string code)
        {
            var update = Builders<Voucher>.Update.Inc(e => e.Quantity, -1);
            await _vouchers.UpdateOneAsync(e => e.Code == code, update);
        }
    }
}
