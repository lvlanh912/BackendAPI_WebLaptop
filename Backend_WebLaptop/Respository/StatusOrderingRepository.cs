using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class StatusOrderingRepository : IStatusOrderingRepository
    {
        private readonly IMongoCollection<StatusOrdering>? StatusOders;

        public StatusOrderingRepository(IDatabase_Service database_Service) 
        {
            StatusOders = database_Service.Get_StatusOrderings_Collection();
        }

        public async Task<bool> DeletebyId(string id)
        {
           var rs= await StatusOders.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount!=0;
        }

        public async Task<bool> Exits(string id)
        {
                var rs =await  StatusOders.FindSync(e => e.Id == id).FirstOrDefaultAsync();
                if (rs == null)
                    return false;
            return true;
        }

        public async Task<List<StatusOrdering>> GetAll() => await StatusOders.FindSync(e=>true).ToListAsync();

        public  async Task<StatusOrdering> GetbyId(string id)=> await StatusOders.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<StatusOrdering> Insert(StatusOrdering e)
        {
            if (!await ValidateData(e))
                throw new Exception("invalid data");
            await StatusOders!.InsertOneAsync(e);
            return e;
        }

        public async Task<bool> Update(StatusOrdering entity)
        {
            var curent = await GetbyId(entity.Id!);
            if (curent == null)
            {
                throw new Exception("This record does not exits");
            }
            entity.Content ??= curent.Content;
            entity.Description = String.IsNullOrWhiteSpace(entity.Description)?curent.Description:entity.Description;
            if (!ValidateData(entity).Result)
                throw new Exception("data invalid");
           var rs= await StatusOders.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return rs!=null;
        }

        private static Task<bool> ValidateData(StatusOrdering entity)
        {
            if (string.IsNullOrEmpty(entity.Content) || string.IsNullOrEmpty(entity.Description))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
