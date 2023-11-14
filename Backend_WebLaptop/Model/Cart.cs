using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
    public class CartItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
