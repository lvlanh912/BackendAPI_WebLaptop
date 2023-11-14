using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment>? _payments;

        public PaymentRepository(IDatabaseService databaseService)
        {
            _payments = databaseService.Get_Payments_Collections();
        }

        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _payments.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await _payments.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            if (rs == null)
                return false;
            return true;
        }

        public async Task<List<Payment>> GetAll() => await _payments.FindSync(e => true).ToListAsync();

        public async Task<Payment> GetbyId(string id) => await _payments.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Payment> Insert(Payment e)
        {
            await _payments!.InsertOneAsync(e);
            return e;
        }

        public async Task<bool> Update(Payment entity)
        {
            var curent = await GetbyId(entity.Id!);
            if (curent == null)
            {
                throw new Exception("This record does not exits");
            }
            entity.Name ??= curent.Name;
            entity.Description = String.IsNullOrWhiteSpace(entity.Description) ? curent.Description : entity.Description;
            var rs = await _payments.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return rs != null;
        }
    }
}
