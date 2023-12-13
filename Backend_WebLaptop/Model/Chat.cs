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
        public DateTime CreateAt { get; set; }= DateTime.Now;
    }
    public class Message
    {
        public bool isAdmin { get; set; } = false;
        public string? Content { get; set; }
        public DateTime TimeSend { get; set; } = DateTime.Now;
    }
}
