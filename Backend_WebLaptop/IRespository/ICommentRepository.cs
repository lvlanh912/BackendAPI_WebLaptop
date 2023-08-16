using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ICommentRepository
    {
        Task<PagingResult<Comment>> GetAll(int pageindex,string ProductId,int size);
        Task<Comment> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<bool> Insert(Comment entity);
        Task<bool> Update(Comment entity);
    }
}
