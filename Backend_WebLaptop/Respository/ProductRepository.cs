using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ICategoryRepository _category;
        private readonly IUploadImageRepository _upload;
        public ProductRepository(IDatabaseService dataBaseService, ICategoryRepository category, IUploadImageRepository upload)
        {
            _products = dataBaseService.Get_Products_Collection();
            _category = category;
            _upload = upload;
        }



        //cập nhật số lượng bán
        public async Task<bool> DecreaseQuantity(List<OrderItem> items)
        {

            // giảm số lượng hàng trong kho, tăng số lượng đã bán của các sản phẩm
            var task = new List<Task>();
            var filter = Builders<Product>.Filter;
            foreach (var item in items)
            {
                var update = Builders<Product>.Update.Inc(e => e.Sold, item.Quantity).Inc(e => e.Stock, -item.Quantity);
                task.Add(_products.FindOneAndUpdateAsync(e => e.Id == item.ProductId, update));
            }
            await Task.WhenAll(task);
            return true;
        }

        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _products.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount > 0;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await _products.FindAsync(e => e.Id == id);
            return rs != null;
        }

        public async Task<PagingResult<Product>> GetAll(string? keywords, string? brand, string? category, int? min, int? max, string sort, int pageindex, int Pagesize)
        {
        //fillter
            //từ khoá
           var result = keywords != null ? await _products.FindSync(e => e.ProductName!.ToLower()!.Contains(keywords.ToLower())).ToListAsync() : await _products.FindSync(e => true).ToListAsync();
            //danh mục -> sản phẩm có trong danh mục đó
            if (category!=null)
                result = result.FindAll(e => e.Categories.Contains(category));
            //hãng
            if (brand != null)
                result = result.FindAll(e => e.BrandName.ToLower().Contains(brand.ToLower()));
            //giá tối thiểu
            if (min != null)
                result = result.FindAll(e => e.Price >= min);
            //giá tối đa
            if (max != null)
                result = result.FindAll(e => e.Price <= max);
            //sort
            result = sort switch
            {
                "date_desc" => result.OrderByDescending(e => e.CreateAt).ToList(),
                "price" => result.OrderBy(e => e.Price).ToList(),
                "price_desc" => result.OrderByDescending(e => e.Price).ToList(),
                "stock_desc" => result.OrderBy(e => e.Stock).ToList(),
                "stock" => result.OrderByDescending(e => e.Stock).ToList(),
                "sold_desc" => result.OrderByDescending(e => e.Sold).ToList(),
                "sold" => result.OrderBy(e => e.Sold).ToList(),
                "view_desc" => result.OrderByDescending(e => e.View).ToList(),
                "view" => result.OrderBy(e => e.View).ToList(),
                _ => result.OrderBy(e => e.CreateAt).ToList()
            };
            return new PagingResult<Product>
            {
                Items = result.Skip((pageindex - 1) * Pagesize).Take(Pagesize),
                PageIndex = pageindex,
                PageSize = Pagesize,
                TotalCount = result.Count
            };
           
            
        }
        public async Task<Product> GetbyId(string id) => await _products.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Product> Insert(ImageUpload<Product> entity)
        {
            entity.Data!.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            if (entity.Images == null)
                throw new Exception("please add image");
            entity.Data.Images = await _upload.UploadProduct_Image(entity);
            entity.Data.CreateAt = DateTime.Now;
            entity.Data.Sold = 0;//đã bán = 0
            entity.Data.View = 0;//lượt xem = 0
            if (await ValidateData(entity.Data))
            {
                await _products.InsertOneAsync(entity.Data);
                return entity.Data;
            }
            throw new Exception("Data invalid");
        }

        public async Task<Product> Update(Product entity)
        {
            var curent = await _products.FindSync(e => entity.Id == entity.Id).FirstOrDefaultAsync();
            entity.CreateAt = curent.CreateAt;
            entity.Sold = (entity.Sold >= 0 ? curent.Sold : entity.Sold);
            entity.Price = (entity.Price <= 1000 ? curent.Price : entity.Price);
            entity.MaxPrice = (entity.MaxPrice <= 1000 ? curent.MaxPrice : entity.MaxPrice);
            entity.View = (entity.View <= 0 ? curent.View : entity.View);
            entity.Categories = (entity.Categories == null || entity.Categories.Count == 0) ? curent.Categories : entity.Categories;
            entity.Stock = entity.Stock <= 0 ? curent.Stock : entity.Stock;
            entity.Weight = entity.Weight <= 0 ? curent.Weight : entity.Weight;
            entity.BrandName = String.IsNullOrWhiteSpace(entity.BrandName) ? curent.BrandName : entity.BrandName;
            entity.Special = (entity.Special == null || entity.Special.Count == 0) ? curent.Special : entity.Special;
            if (await ValidateData(entity))
                return await _products.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
            throw new Exception("Data invalid");
        }
        private async Task<bool> ValidateData(Product entity)
        {
            var list = new List<bool>
            {
                entity.Price>1000,
                entity.Stock>=0,
                entity.Weight>0,
                (entity.Categories.Count>0&&  await _category.Exits(entity.Categories))||entity.Categories.Count==0,
                entity.MaxPrice>1000,
                entity.MaxPrice>entity.Price,
                entity.View>=0,
                entity.Images!=null&&  entity.Images.Count>=1,
                entity.Sold>=0,
                !string.IsNullOrWhiteSpace(entity.ProductName),
                entity.Special!=null&&entity.Special.Count>0
            };
            foreach (var item in list)
                if (item == false)
                    return false;
            return true;
        }

        public async Task<bool> Cansell(List<OrderItem> items)
        {
            //kiểm tra
            foreach (var item in items)
            {

                if (!await Cansell_Item(item.ProductId!, item.Quantity))
                    throw new Exception($"{item.ProductId} không có đủ số lượng để bán");
            }
            return true;
        }
        async Task<bool> Cansell_Item(string id, int quantity)
        {
            var curentProduct = await _products.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            return curentProduct.Stock - quantity >= 0;
        }


    }
}
