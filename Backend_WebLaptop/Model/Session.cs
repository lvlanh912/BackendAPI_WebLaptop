using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend_WebLaptop.Model
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ? AccounId { get; set; }
        public string? Value { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
