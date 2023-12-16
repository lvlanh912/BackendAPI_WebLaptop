using Amazon.Runtime.Internal;
using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Configs.OnlinePayment;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Backend_WebLaptop.Model.OnlinePayment;
using Backend_WebLaptop.Model.OnlinePayment.VNPAY;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Web;
using Timer = System.Timers.Timer;

namespace Backend_WebLaptop.Respository
{

    public class PaymentOnlineRepository : IPaymentOnlineRepository
    {
        private static readonly Dictionary<string, PaymentLinkBag> paymentPending = new();
        private readonly VNPayconfig _VNPayconfig;
        private readonly FrontendConfig _FrontendConfig;
        private readonly IMongoCollection<Order> _order;
        private readonly static Timer timer = new() { Interval = 300 };//5 phút
        public PaymentOnlineRepository(IOptions<VNPayconfig> vnpayconfig, IOptions<FrontendConfig> fronendConfig,
            IDatabaseService database)
        {
            _order = database.Get_Orders_Collection();
            _VNPayconfig = vnpayconfig.Value;
            _FrontendConfig = fronendConfig.Value;
            timer.Elapsed += AutoCleanWaitingPayment!;
        }

        void AutoCleanWaitingPayment(object sender, EventArgs e)
        {
            lock (paymentPending)
            {
                //nếu có danh sách
                if (paymentPending.Count > 0)
                    foreach (var item in paymentPending)
                    {
                        if (item.Value.TimeOut <= DateTime.Now)
                            paymentPending.Remove(item.Key);
                    }
                else
                    timer.Enabled = false;
            }
        }
        public async Task<string> CheckPaymentVNPay(VNPayResponse entity)
        {
            var hash = entity.vnp_SecureHash;
            entity.vnp_TmnCode = _VNPayconfig.Vnp_TmnCode;
            //kiểm tra thuộc tính
            foreach (var item in entity.GetType().GetProperties())
            {
                if (item.GetValue(entity) is null && item.Name != "vnp_BankTranNo")
                    throw new Exception("Yêu cầu không hợp lệ");
            }
            entity.vnp_SecureHash = null;
            var payload = entity.GetQuery();
            //kiểm tra checksum
            if (!entity.ValidateSignature(hash!, _VNPayconfig.Vnp_HashSecret, payload))
                throw new Exception("Lỗi checksum");
            string result;
            var update = Builders<Order>.Update.Set(e => e.IsPaid, true);
            switch (entity.vnp_ResponseCode)
            {
                case "00":
                    result = "Giao dịch thành công";
                    paymentPending.Remove(entity.vnp_TxnRef!);//xoá khỏi danh sách chờ
                    await _order.UpdateOneAsync(e => e.Id== entity.vnp_TxnRef,update);
                    break;
                case "07":
                    result = "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).";
                    paymentPending.Remove(entity.vnp_TxnRef!);//xoá khỏi danh sách chờ
                    break;
                case "09":
                    result = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.";
                    break;
                case "10":
                    result = "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
                    break;
                case "11":
                    result = "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
                    paymentPending.Remove(entity.vnp_TxnRef!);//xoá khỏi danh sách chờ
                    break;
                case "12":
                    result = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
                    paymentPending.Remove(entity.vnp_TxnRef!);//xoá khỏi danh sách chờ
                    break;
                case "13":
                    result = "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.";
                    break;
                case "24":
                    result = "Giao dịch không thành công do: Khách hàng hủy giao dịch";
                    paymentPending.Remove(entity.vnp_TxnRef!);//xoá khỏi danh sách chờ
                    break;
                case "51":
                    result = "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                    break;
                case "65":
                    result = "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                    break;
                case "75":
                    result = "Ngân hàng thanh toán đang bảo trì.";
                    break;
                case "79":
                    result = "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch";
                    break;
                default:
                    result = "Giao dịch không thành công, liên hệ dịch vụ CSKH để được hỗ trợ";
                    break;
            }
            return result;
        }

    public Task<string> CreatePaymentLinkVNPay(Order order,string ipAddress)
        {
            var VNPayRequest = new VNPayRequest
            {
                vnp_Amount = (order.Paid * 100),
                vnp_CurrCode = "VND",
                vnp_Locale = "vn",
                vnp_Command = "pay",
                vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
                vnp_IpAddr = ipAddress,
                vnp_OrderInfo = $"Thanh toan don hang {order.Id} DientuViu",
                vnp_ReturnUrl = _FrontendConfig.Host + _FrontendConfig.ResultPaymentPage,
                vnp_TmnCode = _VNPayconfig.Vnp_TmnCode,
                vnp_TxnRef = order.Id,
                vnp_Version = _VNPayconfig.Vnp_Version
            };
            var query = VNPayRequest.GetQuery();
            var result = _VNPayconfig.Vnp_Url + "?" + query + "&vnp_SecureHash=" + VNPayRequest.HmacSHA512(_VNPayconfig.Vnp_HashSecret, query);
            //Thêm link thanh toán vào danh sách chờ
            paymentPending.Add(order.Id!, new PaymentLinkBag
            {
                PaymentLink = result
            });
            // khởi động dọn dẹp
            if (!timer.Enabled)
                timer.Enabled = true;
            return Task.FromResult(result);

        }

        public async Task<string> RegetLinkVNPay(string orderId,string accountId,string IpAddress)
        {
           //kiểm tra trong danh sách chờ còn không
           if(paymentPending.TryGetValue(orderId,out var Link))
              return  Link.PaymentLink;
           else//tạo link mới
           {
                var order = await _order.FindSync(e => e.Id == orderId&&e.AccountId==accountId).FirstOrDefaultAsync() 
                    ?? throw new Exception("Không tồn tại đơn hàng này");

                return await CreatePaymentLinkVNPay(order, IpAddress);


           }    

            
                
        }
    }
   
}
