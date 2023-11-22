using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Name_Product")]
        public string? ProductName { get; set; }
        public Int32 Price { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Product_CategoriesID")]
        public List<string> Categories { get; set; } = new List<string>();
        [BsonElement("Max_Price")]
        public Int32? MaxPrice { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public List<string>? Images { get; set; }

        public Int32 Stock { get; set; }

        public Int32 Sold { get; set; } = 0;

        public Int64 View { get; set; } = 1;

        [BsonElement("Brand_name")]
        public string BrandName { get; set; } = string.Empty;

        public double? Weight { get; set; }

        public List<Speacial> Special { get; set; } = new List<Speacial>();
    }
    public class Speacial
    {
        public string? K { get; set; }
        public string? V { get; set; }
    }
}
