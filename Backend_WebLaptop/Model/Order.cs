using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public Payment? PaymentMethod { get; set; }//= null là thanh toán khi nhận hàng
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Shipping? Status { get; set; } = new Shipping(1);
        public ShippingAddress? ShippingAddress { get; set;}
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public int Total { get; set; } = 0;
        public int Paid { get; set; } = 0;
        public bool IsPaid { get; set; } = false;
        public Voucher? Voucher { get; set; }
    }
    public class OrderItem
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
    /// <summary>
    /// Infor status order
    /// </summary>
    public class Shipping
    {
        public int? Code { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdateAt { get; set; }

        /// <summary>
        /// Create Status Shipping.
        /// </summary>
        /// <returns>
        /// A new Shipping Status.
        /// <para>
        /// type =0, Order Canceled
        /// </para>
        ///  <para>
        /// type =1, Order Wait to confirm
        /// </para>
        /// <para>
        /// type=2, Order is Shipping
        /// </para>
        /// <para>
        /// type=3, Order is Success
        /// </para>
        /// </returns>
        /// 
        public Shipping(int type)
        {
            if (type < 0 || type > 3)
                throw new Exception("Not have this type");
            switch (type)
            {
                default:
                    this.Code = 0;
                    this.Description = "Đơn hàng đã bị huỷ";
                    this.UpdateAt = DateTime.Now;
                    break;
                case 1:
                    this.Code = type;
                    this.Description = "Đơn hàng chờ xác nhận";
                    this.UpdateAt = DateTime.Now;
                    break;
                case 2:
                    this.Code = type;
                    this.Description = "Đơn hàng đã xác nhận, Đang giao hàng";
                    this.UpdateAt = DateTime.Now;
                    break;
                case 3:
                    this.Code = type;
                    this.Description = "Đã giao hàng thành công";
                    this.UpdateAt = DateTime.Now;
                    break;
            }
        }
    }
}
