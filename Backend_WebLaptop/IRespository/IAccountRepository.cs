using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IAccountResposytory
    {
        Task<PagingResult<Account>> GetAll(string? keywords, int pageindex, int pagesize);
        Task<Account> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Account> Insert(ImageUpload<Account> entity);
        Task<Account> Update( ImageUpload<Account> entity);
        Task<bool> Exits(string id);
        Task<bool> ExitsByUserName(string username);
    }
}
