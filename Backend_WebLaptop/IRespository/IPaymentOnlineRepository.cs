using Backend_WebLaptop.Model;
using Backend_WebLaptop.Model.OnlinePayment.VNPAY;

namespace Backend_WebLaptop.IRespository
{
    public interface IPaymentOnlineRepository
    {
        Task<string> CreatePaymentLinkVNPay(Order order,string ipAddress);
        /// <summary>
        /// Test create link
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
      /*  Task<string> CreatePaymentLinkVNPayTest(string IP, string id);*/
        
        Task<string> CheckPaymentVNPay(VNPayResponse entity);
        Task<string> RegetLinkVNPay(string orderId, string accountId,string IpAddress);
    }
}
