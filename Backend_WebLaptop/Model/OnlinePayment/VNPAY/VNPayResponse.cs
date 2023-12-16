using System.Net;

namespace Backend_WebLaptop.Model.OnlinePayment.VNPAY
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class VNPayResponse
    {
        public string? vnp_Amount { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_BankTranNo { get; set; }
        public string? vnp_CardType { get; set; }
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_PayDate { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_SecureHash { get; set; }
        public string? vnp_TmnCode { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? vnp_TransactionStatus { get; set; }
        public string? vnp_TxnRef { get; set; }

        public string GetQuery()
        {
            var listkeyvalue = new List<string>();
            foreach (var item in GetType().GetProperties())
                if (item.GetValue(this) is not null)
                    listkeyvalue.Add(WebUtility.UrlEncode(item.Name) + "=" + WebUtility.UrlEncode(item.GetValue(this)!.ToString()));//không encode->Lỗi chữ ký
            listkeyvalue.Sort();
            string result = string.Join('&', listkeyvalue);
            return result;
        }
        public bool ValidateSignature(string inputHash, string secretKey, string payload)
        {
            string myChecksum = VNPayRequest.HmacSHA512(secretKey, payload);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
