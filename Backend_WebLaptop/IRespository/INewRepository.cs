using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface INewRepository
    {
        Task<PagingResult<News>> GetAll(string? keywords, DateTime? startdate, DateTime? enddate, int pageindex=1, int pagesize=25, string? sort="date_desc");
        Task<News> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<News> Insert(ImageUpload<News> entity);
        Task<News> Update(ImageUpload<News> entity);
    }
}
