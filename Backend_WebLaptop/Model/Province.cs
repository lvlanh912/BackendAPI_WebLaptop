using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend_WebLaptop.Model
{
    public class Province
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int? CODE { get; set; }
        public string? Name { get; set; }


    }
}
