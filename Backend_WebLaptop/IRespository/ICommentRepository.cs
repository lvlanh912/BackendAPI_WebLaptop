using Backend_WebLaptop.Model;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Backend_WebLaptop.IRespository
{
    public interface ICommentRepository
    {
        Task<PagingResult<Comment>> GetAll( string? accountid,string? productId,string? keywords,string sort,int pageindex,int pagesize);
        Task<Comment> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Comment> Insert(Comment entity);
        Task<bool> Update(Comment entity);
        Task<long> GetTotalCreatebyTime(DateTime start, DateTime end);
    }
}
