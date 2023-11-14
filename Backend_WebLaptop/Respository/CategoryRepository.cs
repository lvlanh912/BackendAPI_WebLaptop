using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category>? _categories;

        public CategoryRepository(IDatabaseService databaseService)
        {
            _categories = databaseService.Get_Categories_Collection();
        }

        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _categories.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
        }

        public async Task<bool> Exits(List<string> listId)
        {
            //nếu 1 catogery trong danh sách không tồn tại=>false
            foreach (var id in listId)
            {
                var rs = await _categories.FindSync(e => e.Id == id).FirstOrDefaultAsync();
                if (rs == null)
                    return false;
            }
            return true;
        }

        public async Task<List<Category>> GetAll() => await _categories.FindSync(e => true).ToListAsync();

        public async Task<Category> GetbyId(string id) => await _categories.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Category> Insert(Category e)
        {
            if (await ValidateData(e))
            {
                if(e.Childs.Count>1)
                    e.Childs= await CleanData(e.Childs);
                await _categories!.InsertOneAsync(e);
            }
               
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
           // entity.Description ??= curent.Description;
            var rs = await _categories.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return rs != null;
        }

        private Task<bool> ValidateData(Category entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
        //loại bỏ trùng lặp id trong mảng child
        private Task<List<string>> CleanData(List<string> listId)=> Task.FromResult(listId.Distinct().ToList());
    }
}
