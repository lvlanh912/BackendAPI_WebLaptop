using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IProductRepository
    {
        Task<PagingResult<Product>> GetAll(string? keywords, string? brand,string? category, int? min, int? max,string sort, int pageindex, int Pagesize);
        Task<List<Product>> GetbyKeyword(string keywords);
        Task<Product> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Product> Insert(ImageUpload<Product> entity);
        Task<Product> Update(ImageUpload<Product> entity);
        Task<bool> Exits(string id);
        Task<bool> DecreaseQuantity(List<OrderItem> items);
        Task<List<OrderItem>> Cansell(List<OrderItem> items);
    }
}
