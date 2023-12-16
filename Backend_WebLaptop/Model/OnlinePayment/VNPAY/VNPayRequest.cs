using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Backend_WebLaptop.Model.OnlinePayment.VNPAY
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class VNPayRequest
    {


        public decimal? vnp_Amount { get; set; }
        public string vnp_Command { get; set; } = "pay";
        public string vnp_CreateDate { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public string? vnp_CurrCode { get; set; } = "VND";
        public string? vnp_BankCode { get; set; }
        public string? vnp_IpAddr { get; set; }
        public string vnp_Locale { get; set; } = "vn";
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_OrderType { get; set; } = "other";
        public string? vnp_ReturnUrl { get; set; }
        public string? vnp_IpnUrl { get; set; }
        public string? vnp_TmnCode { get; set; }
        // public string?  vnp_ExpireDate { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_Version { get; set; }
        public string? vnp_SecureHash { get; set; }

        //sắp xếp các thuộc tính và giá trị theo Alphabet
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
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }
}
