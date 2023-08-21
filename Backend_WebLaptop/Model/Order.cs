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
        public string? Payment_methodID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CartID { get; set; }
        public List<OrderStatus>? Status { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Shipping_AddressID { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Total { get; set; }
        public int Paid { get; set; } = 0;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? VoucherID { get; set; }
    }
    public class OrderStatus
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? StatusID { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
