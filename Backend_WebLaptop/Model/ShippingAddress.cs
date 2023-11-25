using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class ShippingAddress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("AccountID")]
        public string? AccountId { get; set; }
        [BsonElement("Full_name")]
        public string Fullname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [BsonElement("Phone_number")]
        public Int32 Phone { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Ward_ID")]
        public string? WardId { get; set; }
    }
}
