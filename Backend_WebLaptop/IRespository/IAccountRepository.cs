using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IAccountRepository
    {
        Task<PagingResult<Account>> GetAll(string? keywords,string? type, DateTime? startdate, DateTime? enddate, int? role, bool? gender, int pageindex, int pagesize, string sort);
        Task<Account> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Account> Insert(ImageUpload<Account> entity);
        Task<Account> Update(ImageUpload<Account> entity);
        Task<bool> Exits(string id);
        Task<bool> ExitsByUserName(string username);
        Task<int> GetTotalOrder(string accountId);
        Task<int> GetTotalComment(string accountId);
        Task<long> GetTotalCreatebyTime(DateTime start, DateTime end);
        Task UpdateImage(ImageUpload<Account> entity);
        Task Updateinfor(Account entity);
        Task<Account> GetPublicInfor(string accountId);
    }
}
