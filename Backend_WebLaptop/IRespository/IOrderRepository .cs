using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IOrderRepository
    {
        //tạo Order mới
        Task<Order> CreateOrder(Order entity,bool isAdmin);

        Task<Order> GetOrderbyId(string id);

        Task<PagingResult<Order>> GetAllOrders(string? accountid, int? status, bool? isPaid, string? paymentId,
            int? minPaid, int? maxPaid, DateTime? startdate, DateTime? enddate, string sort, int pagesize = 25, int pageindex = 1);

        Task<Order> EditOrder(Order entity, string id);

        Task<bool> DeleteOrder(string id);

        Task<Order> UpdateStatus(string id);

        Task<long> Get_toltalSell(DateTime? start, DateTime? end);

        Task<Order> Checkout(Order entity);
    }
}
