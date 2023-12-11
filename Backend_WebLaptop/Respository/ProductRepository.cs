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
                task.Add(_products.FindOneAndUpdateAsync(e => e.Id == item.Product!.Id, update));
            }
            await Task.WhenAll(task);
            return true;
        }

        public async Task<bool> DeletebyId(string id)
        {
            
            var curent = await GetbyId(id);
            if (curent != null)
            {
                //Xoá ảnh
                curent.Images.ForEach(async (item) => await _upload.Delete_Image(2, item));
                var rs = await _products.DeleteOneAsync(e => e.Id == id);
                return rs.DeletedCount > 0;
            }
            throw new Exception("Product is exist");
           
           
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await _products.FindAsync(e => e.Id == id);
            return rs != null;
        }

        public async Task<PagingResult<Product>> GetAll(string? keywords, bool? stock, string? brand, string? category, int? min, int? max, string sort, int pageindex, int Pagesize)
        {
        //fillter
            //từ khoá
           var result = keywords != null ? await _products.FindSync(e => e.ProductName!.ToLower()!.Contains(keywords.ToLower())).ToListAsync() : await _products.FindSync(e => true).ToListAsync();
            //danh mục -> sản phẩm có trong danh mục đó
            if (category!=null)
                result = result.FindAll(e => e.Categories.Contains(category));
            //hãng
            if (brand != null)
                result = result.FindAll(e => e.BrandName.Contains(brand.Trim().ToUpper()));
            //giá tối thiểu
            if (min != null)
                result = result.FindAll(e => e.Price >= min);
            //giá tối đa
            if (max != null)
                result = result.FindAll(e => e.Price <= max);
            //tình trạng hàng
            if (stock != null)
                result = stock==true ? result.FindAll(e => e.Stock > 0) : result.FindAll(e => e.Stock == 0);

            //sort
            result = sort switch
            {
                "name" => result.OrderBy(e => e.ProductName).ToList(),
                "name_desc" => result.OrderByDescending(e => e.ProductName).ToList(),
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
            entity.Data.BrandName = entity.Data.BrandName.TrimEnd().TrimStart().ToUpper();
            if (await ValidateData(entity.Data))
            {
                await _products.InsertOneAsync(entity.Data);
                return entity.Data;
            }
            throw new Exception("Data invalid");
        }

        public async Task<Product> Update(ImageUpload<Product> entity)
        {
            var curent = await _products.FindSync(e =>e.Id == entity.Data!.Id).FirstOrDefaultAsync();
            entity.Data!.CreateAt = curent.CreateAt;
            entity.Data.Sold = (entity.Data.Sold < 0 ? curent.Sold : entity.Data.Sold);
            entity.Data.Price = (entity.Data.Price <= 1000 ? curent.Price : entity.Data.Price);
            entity.Data.MaxPrice = (entity.Data.MaxPrice <= 1000 ? curent.MaxPrice : entity.Data.MaxPrice);
            entity.Data.View = (entity.Data.View <= 0 ? curent.View : entity.Data.View);
            entity.Data.Categories = (entity.Data.Categories == null || entity.Data.Categories.Count == 0) ? curent.Categories : entity.Data.Categories;
            entity.Data.Stock = entity.Data.Stock < 0 ? curent.Stock : entity.Data.Stock;
            entity.Data.Weight = entity.Data.Weight <= 0 ? curent.Weight : entity.Data.Weight;
            entity.Data.BrandName = String.IsNullOrWhiteSpace(entity.Data.BrandName) ? curent.BrandName : entity.Data.BrandName;
            entity.Data.Special = (entity.Data.Special == null || entity.Data.Special.Count == 0) ? curent.Special : entity.Data.Special;
            if (entity.Images==null||entity.Images.Count==0)
                throw new Exception("please add image");
            //xoá ảnh cũ
             curent.Images.ForEach(async (item) => await _upload.Delete_Image(2, item));
            //up dảnh mới
            entity.Data.Images = await _upload.UploadProduct_Image(entity);
            if (await ValidateData(entity.Data))
                return await _products.FindOneAndReplaceAsync(e => e.Id == entity.Data.Id, entity.Data);
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
                entity.Special!=null&&entity.Special.Count>=0
            };
            foreach (var item in list)
                if (item == false)
                    return false;
            return true;
        }

     public async Task<List<OrderItem>> Cansell(List<OrderItem> items)
        {
            var result = new List<OrderItem>();
            //kiểm tra
            foreach (var item in items)
            {
                var product = await Cansell_Item(item.Product!.Id!, item.Quantity) ?? throw new Exception("Một số sản phẩm không đủ số lượng để bán");
                result.Add(new OrderItem()
                {
                    Product = product,
                    Quantity = item.Quantity
                }) ;
            }
            return result;
        }
        async Task<Product> Cansell_Item(string id, int quantity)
        {
            var curentProduct = await _products.FindSync(e => e.Id == id&&e.Stock>=quantity).FirstOrDefaultAsync();
            return curentProduct;
        }
        public async Task<List<Product>> GetbyKeyword(string keywords)
        {
          var filter=  Builders<Product>.Filter.Regex(e => e.ProductName, new BsonRegularExpression($".*{keywords}"));
            var result =await _products.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task RestoreItem(string productid, int quantity)
        {
            var update = Builders<Product>.Update.Inc(e => e.Sold, -1).Inc(e => e.Stock, -1);
           await _products.UpdateOneAsync(e => e.Id == productid, update);
        }

        public async Task InsertView(string productid)
        {
            var update = Builders<Product>.Update.Inc(e => e.View, 1);
            await _products.UpdateOneAsync(e => e.Id == productid, update);
        }
        public async Task<long> GetTotalOutStock() => await _products.CountDocumentsAsync(e => e.Stock<=0);

    }
}
