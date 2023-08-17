using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IProductRepository
    {
        Task<PagingResult<Product>> GetAll(ProductFilter? filter,int pageindex, int pagesize);//get all products
        Task<Product> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<bool> Insert(ImageUpload<Product> entity);
        Task<bool> Update(Product entity);
        Task<bool> Exits(string id);
    }
}
