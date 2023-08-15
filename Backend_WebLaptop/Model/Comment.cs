using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public byte Star { get; set; } = 5;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountID { get; set; }
        public string? Conntent { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
