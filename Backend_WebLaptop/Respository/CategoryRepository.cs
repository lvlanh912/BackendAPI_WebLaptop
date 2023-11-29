using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;

namespace Backend_WebLaptop.Respository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category>? _categories;
        private readonly IMongoCollection<Product>? _products;

        public CategoryRepository(IDatabaseService databaseService)
        {
            _categories = databaseService.Get_Categories_Collection();
            _products = databaseService.Get_Products_Collection();
        }

        public async Task<bool> DeletebyId(string id)
        {
            var current = await GetbyId(id) ?? throw new Exception("Category is does not exist");
            //xoá trong childs của category cha (nếu có)
            if (current.ParentCategoryId != null)
            {
                var parentCat = await GetbyId(current.ParentCategoryId);
                parentCat.Childs.Remove(current.Id!);
                var update = Builders<Category>.Update.Set(e => e.Childs, parentCat.Childs);
                _categories.UpdateOne(e => e.Id == parentCat.Id, update);
            }
            //thay đổi cat con thành danh mục gốc (nếu có cat con)
            if (current.Childs.Count > 0)
            {
                var updateTasks = current.Childs.Select(item =>
                {
                    var update = Builders<Category>.Update.Set(e => e.ParentCategoryId, null);
                    return _categories.UpdateOneAsync(e => e.Id == item, update);
                });
               await Task.WhenAll(updateTasks);
            }
            //Xoá category ra khỏi các sản phẩm 
            var update_Products = Builders<Product>.Update.Pull(e => e.Categories, id);
            await _products.UpdateManyAsync(e => e.Categories!.Count>0&& e.Categories.Contains(id), update_Products);
            var rs = await _categories.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
        }

        public async Task<bool> Exits(string id)
        {
            //nếu 1 catogery trong danh sách không tồn tại=>false
                var rs = await _categories.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            return rs != null;
        }

       

        public async Task<PagingResult<Category>> GetAll(string? ParentCategoryId, string? keywords, string sort, int pageindex, int pagesize) 
        {
            var result = new List<Category>();
            //lấy danh sách tất cả danh mục gốc (không có danh mục cha)
            if(ParentCategoryId == null)
                 result=keywords!=null? await _categories.FindSync( e => e.Name.ToLower().Contains(keywords.ToLower())&&e.ParentCategoryId==null).ToListAsync(): await _categories.FindSync(e => e.ParentCategoryId == null).ToListAsync();
            else
                //lấy danh sách các danh mục con 
                result = keywords != null ? await _categories.FindSync(e =>e.ParentCategoryId== ParentCategoryId && e.Name!.Contains(keywords)).ToListAsync() : await _categories.FindSync(e => e.ParentCategoryId == ParentCategoryId).ToListAsync();
            //sort
            if (sort == "name_desc")
                result = result.OrderByDescending(e => e.Name).ToList();
            else
                result = result.OrderBy(e => e.Name).ToList();
            return new PagingResult<Category>
            {
                Items = result.Skip((pageindex - 1) * pagesize).Take(pagesize),
                PageSize = pagesize,
                PageIndex = pageindex,
                TotalCount = result.Count
            };
        }

        public async Task<Category> GetbyId(string id)
        {
          var rs=  await _categories.FindSync(e => e.Id == id).FirstOrDefaultAsync();
          return  rs ?? throw new Exception("Not valid this Catogery");
        }

        public async Task<Category> Insert(Category entity)
        {
            if (await ValidateData(entity))
            {
                await _categories!.InsertOneAsync(entity);
                //thêm ID con vào mảng childs của category cha
                if (entity.ParentCategoryId != null)
                {
                    var update = Builders<Category>.Update.Push(e => e.Childs, entity.Id);
                    _categories.UpdateOne(e => e.Id == entity.ParentCategoryId, update);                
                }
            }
            return entity;
        }

        public async Task<bool> Update(Category entity)
        {
            
            if (entity.ParentCategoryId!=null&& !await Exits(entity.ParentCategoryId))
                throw new Exception("ParentCategoryId not exits");
            var curent = await GetbyId(entity.Id!) ?? throw new Exception("This record does not exist");
            entity.Name ??= curent.Name;
            entity.Childs ??= curent.Childs;
            var rs = await _categories.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            // var update_delChildId = Builders<Category>.Update.Set(e => e.Name , entity.Name);
            if (curent.ParentCategoryId != entity.ParentCategoryId)
            {
                //thay đổi cat cha cũ
                if (curent.ParentCategoryId != null)
                {
                    //xoá id của cat sửa đổi trong childs của cat cha cũ(chưa hoạt động)
                    var curentParent = await GetbyId(curent.ParentCategoryId);
                    var list_childs = curentParent.Childs;
                    list_childs.Remove(entity.Id!);
                    var update_delChildId = Builders<Category>.Update.Set(e=>e.Childs, list_childs);
                    await _categories.UpdateOneAsync(e=>e.Id==curent.ParentCategoryId, update_delChildId);
                }
               //thay đổi cat cha mới
                if (entity.ParentCategoryId != null)
                {
                    //Thêm vào childs cat cha mới (nếu có cat cha mới)
                    var update_delChildId = Builders<Category>.Update.Push(e => e.Childs, entity.Id);
                    await _categories.UpdateOneAsync(e => e.Id == entity.ParentCategoryId, update_delChildId);
                }
            }
            return rs != null;
        }

        private async Task<bool> ValidateData(Category entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
                throw new Exception("Tên danh mục không được để trống");
            if( entity.ParentCategoryId!=null && !await Exits(entity.ParentCategoryId))
                throw new Exception("Không tồn tại danh mục cha");
            return true;
        }
        //loại bỏ trùng lặp id trong mảng child
        public async Task<bool> Exits(List<string> id)
        {
            foreach(var item in id)
            {
                if(!await Exits(item))
                    return false;
            }
            return true;
        }

        public async Task<List<Category>> GetAllCategorybyName(string name)=> await _categories.FindSync(e => e.Name!.ToLower().Contains(name.ToLower())).ToListAsync();

        public async Task<List<Category>> GetListChildsById(string parentID)
        {
            return await _categories.FindSync(e => e.ParentCategoryId == parentID).ToListAsync();
        }
    }
}
