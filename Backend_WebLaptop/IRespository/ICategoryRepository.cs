using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();
        Task<Category> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Category> Insert(Category entity);
        Task<bool> Update(Category entity);
        Task<bool> Exits(List<string> id);
    }
}
