using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IOrderRepository
    {
        //tạo Order mới
        Task<Order> CreateOrder(Order entity);

        Task<Order> GetOrderbyId(string id);

        Task<PagingResult<Order>> GetAllOrders(string? UserId, string? keywords, string? paymentID, int PageSize,
           int pageindex, int start, int end);

        Task<Order> EditOrder(Order entity, string voucherId);

        Task<bool> DeleteOrder(string id);

        Task<Order> UpdateStatus(string id);

        Task<long> Get_toltalSell(DateTime? start,DateTime? end);

        //Task<Order> Checkout(Payment entity);
    }
}
