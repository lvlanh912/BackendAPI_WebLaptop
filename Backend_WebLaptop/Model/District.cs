using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class District
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int? CODE { get; set; }
        public string? Name { get; set; }
        public int ProvinceCode { get; set; }
    }
}
