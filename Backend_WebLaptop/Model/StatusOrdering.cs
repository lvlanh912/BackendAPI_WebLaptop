using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend_WebLaptop.Model
{
    public class StatusOrdering
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
    }
}
