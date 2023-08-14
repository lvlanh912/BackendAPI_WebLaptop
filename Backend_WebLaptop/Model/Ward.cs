using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Ward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int? DistrictCode { get; set; }
    }
}
