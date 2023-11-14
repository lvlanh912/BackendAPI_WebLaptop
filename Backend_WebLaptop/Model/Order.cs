using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Payment_methodId")]
        public string? PaymentMethodId { get; set; }//= null là thanh toán khi nhận hàng
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public List<OrderItem>? Items { get; set; }
        public List<Shipping>? Status { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Total { get; set; }
        public int? Paid { get; set; }
        public string? VoucherCode { get; set; }
    }
    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class Shipping
    {
        public string? Description { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
