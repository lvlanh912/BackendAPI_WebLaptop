using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ICategoryRepository
    {
        Task<PagingResult<Category>> GetAll(string? ParentCategoryId, string? keywords, string sort, int pageindex , int pagesize);
        Task<List<Category>> GetAllCategorybyName(string name);
        Task<Category> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Category> Insert(Category entity);
        Task<bool> Update(Category entity);
        Task<bool> Exits(List<string> id);
    }
}
