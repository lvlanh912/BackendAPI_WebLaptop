using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _Products;
        private readonly ICategoryRepository _Category;
        private readonly IUploadImageRepository _Upload ;
        public ProductRepository(IDatabase_Service dataBase_Service, ICategoryRepository category, IUploadImageRepository upload)
        {
            _Products = dataBase_Service.Get_Products_Collection();
            _Category = category;
            _Upload = upload;
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
                task.Add(_Products.FindOneAndUpdateAsync(e => e.Id == item.ProductID, update));
            }
            await Task.WhenAll(task);
            return true;
        }

        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _Products.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount > 0;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await _Products.FindAsync(e => e.Id == id);
            return rs != null;
        }

        public async Task<PagingResult<Product>> GetAll(ProductFilter? filter, int pageindex, int pagesize)
        {
            return new PagingResult<Product>
            {
                Items = await GetProducts(filter!),
                PageIndex = pageindex,
                PageSize = pagesize
            };
        }
        public async Task<Product> GetbyId(string id)=>  await _Products.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<bool> Insert(ImageUpload<Product> entity)
        {
            entity.data!.Id= ObjectId.GenerateNewId(DateTime.Now).ToString();
            if (entity.images == null)
                throw new Exception("please add image");
            entity.data.Images =await _Upload.UploadProduct_Image(entity);
            entity.data.CreateAt = DateTime.Now;
            entity.data.Sold = 0;//đã bán = 0
            entity.data.View = 0;//lượt xem = 0
            if (await ValidateData(entity.data))
            {
                await _Products.InsertOneAsync(entity.data);
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Product entity)
        {
            var curent =await _Products.FindSync(e => entity.Id == entity.Id).FirstOrDefaultAsync();
            entity.CreateAt = curent.CreateAt;
            entity.Sold =(entity.Sold>=0?curent.Sold:entity.Sold);
            entity.Price=(entity.Price<=1000?curent.Price:entity.Price);
            entity.MaxPrice=(entity.MaxPrice<=1000?curent.MaxPrice:entity.MaxPrice);
            entity.View=(entity.View<=0?curent.View:entity.View);
            entity.CategoryID=(entity.CategoryID==null||entity.CategoryID.Count==0)?curent.CategoryID:entity.CategoryID;
            entity.Stock=entity.Stock<=0?curent.Stock:entity.Stock;
            entity.Weight=entity.Weight<=0?curent.Weight:entity.Weight;
            entity.BrandName=String.IsNullOrWhiteSpace(entity.BrandName)?curent.BrandName:entity.BrandName;
            entity.Special = (entity.Special == null || entity.Special.Count == 0) ? curent.Special : entity.Special;
            if(await ValidateData(entity))
            {
               var rs=await _Products.ReplaceOneAsync(e => e.Id == entity.Id,entity);
                return rs.ModifiedCount > 0;
            }
            return false;
        }
        private async Task<IEnumerable<Product>> GetProducts(ProductFilter filter)
        {
            var rs = Enumerable.Empty<Product>();
            //nếu có từ khoá và 1 danh mục ->lọc sản phẩm có từ khoá trong danh mục đấy
            if (!string.IsNullOrEmpty(filter.Keywords)&&!string.IsNullOrEmpty(filter.Category))
            {
                var data = await _Products.FindAsync(e => e.CategoryID!.Contains(filter.Category)&&
                e.ProductName!.Trim().ToLower().Contains(filter.Keywords));
                rs =  data.ToEnumerable<Product>();
            }
            //nếu chỉ có danh mục ->sản phẩm có trong danh mục đó
            if (string.IsNullOrEmpty(filter.Keywords) && !string.IsNullOrEmpty(filter.Category))
            {
                var data = await _Products.FindAsync(e => e.CategoryID!.Contains(filter.Category));
                rs = data.ToEnumerable<Product>();
            }
           //nếu có danh mục và hãng-> lọc Sản phẩm theo hãng trong danh mục đó
            if(!string.IsNullOrEmpty(filter.Brand) && !string.IsNullOrEmpty(filter.Category))
            {
                var data =await  _Products.FindAsync(e => e.CategoryID!.Contains(filter.Category) &&
                e.BrandName!.Trim().ToLower()==filter.Brand.Trim().ToLower()   );
                rs = data.ToEnumerable<Product>();
            }
            //nếu chỉ có hãng-> lọc tất cả sản phẩm theo hãng đó
            if (!string.IsNullOrEmpty(filter.Brand) && string.IsNullOrEmpty(filter.Category))
            {
                var data = await _Products.FindAsync(e =>e.BrandName!.Trim().ToLower() == filter.Brand.Trim().ToLower());
                rs = data.ToEnumerable<Product>();
            }
            if (filter.Min_price.HasValue)
                rs=rs.Where(e=>e.Price>=filter.Min_price);
            if (filter.Max_price.HasValue)
                rs = rs.Where(e => e.Price <= filter.Max_price);
            return rs;
        }
        private async Task<bool> ValidateData(Product entity)
        {
            var list = new List<bool>
            {
                entity.Price>1000,
                entity.Stock>=0,
                entity.Weight>0,
                entity.CategoryID!.Count>0&&  await _Category.Exits(entity.CategoryID),
                entity.MaxPrice>1000,
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

               if(!await Cansell_Item(item.ProductID!,item.Quantity))
                    throw new Exception($"{item.ProductID} không có đủ số lượng để bán");
            }
            return true;
        }
        async Task<bool>Cansell_Item (string id, int quantity)
        {
            var curent_product = await _Products.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            return curent_product.Stock - quantity >= 0;
        }
    }
}
