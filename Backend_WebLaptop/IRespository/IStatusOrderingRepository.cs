using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IStatusOrderingRepository
    {
        Task<List<StatusOrdering>> GetAll();
        Task<StatusOrdering> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<StatusOrdering> Insert(StatusOrdering entity);
        Task<bool> Update(StatusOrdering entity);
        Task<bool> Exits(string id);
    }
}
