using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend_WebLaptop.Model
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
    public class Message
    {
        string? Name { get; set; }
        string? Content { get; set; }
    }
}
