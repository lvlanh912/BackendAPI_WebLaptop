using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IOrderRepository
    {
        //tạo Order mới
        /// <summary>
        /// Validate Data
        /// Type order:Default is customer Equal true is admin Create Order
        /// </summary>
        /// <param name="entity"> Order</param>
        /// <param name="type"> </param>
        /// <returns></returns>
        Task<Order> CreateOrder(Order entity,bool isAdmin);

        Task<Order> GetOrderbyId(string id);

        Task<PagingResult<Order>> GetAllOrders(string? accountid, int? status, bool? isPaid, string? paymentId,
            int? minPaid, int? maxPaid, DateTime? startdate, DateTime? enddate, string sort, int pagesize = 25, int pageindex = 1);

        Task<bool> EditOrder(string id,int? status,ShippingAddress? shippingAddress,bool? ispaid);

        Task<bool> DeleteOrder(string id);

        Task<long> Get_toltalSell(DateTime? start, DateTime? end);
        Task<long> Get_CountPending();
        Task CancelOrder(string userId, string orderId);

        Task<Order> Checkout(Order entity);

        Task<PagingResult<Order>> GetMyOrder(string accountId,int? type, int pagesize , int pageindex);

        Task<bool> IsBought(string productId,string accountid);
    }
}
