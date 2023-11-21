using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Name_Category")]
        public string? Name { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ParentCategoryId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ChildCategory")]
        public List<string> Childs { get; set; } = new List<string>();
    }
}
