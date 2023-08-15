using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category>? Categories;

        public CategoryRepository(IDatabase_Service database_Service) 
        {
            Categories = database_Service.Get_Categories_Collection();
        }

        public async Task<bool> DeletebyId(string id)
        {
           var rs= await Categories.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount!=0;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await Categories.FindAsync(e => e.Id == id);
            return rs == null;
        }

        public async Task<List<Category>> GetAll() => await Categories.FindSync(e=>true).ToListAsync();

        public  async Task<Category> GetbyId(string id)=> await Categories.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Category> Insert(Category e)
        {
            if( await ValidateData(e))
            await Categories!.InsertOneAsync(e);
            return e;
        }

        public async Task<bool> Update(Category entity)
        {
            var curent = await GetbyId(entity.Id!);
            if (curent == null)
            {
                throw new Exception("This record does not exits");
            }
            entity.Name ??= curent.Name;
            entity.Description ??= curent.Description;
           var rs= await Categories.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return rs!=null;
        }

        private static Task<bool> ValidateData(Category entity)
        {
            if (string.IsNullOrEmpty(entity.Name) || string.IsNullOrEmpty(entity.Description))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
