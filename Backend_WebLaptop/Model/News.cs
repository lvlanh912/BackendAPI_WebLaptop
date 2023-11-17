using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend_WebLaptop.Model
{
    public class News
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Title { get; set; }
        public List<string>? Images { get; set; }
        public string? content { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
