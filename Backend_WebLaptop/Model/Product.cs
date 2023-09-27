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
        [BsonElement("Max_Price")]
        public Int32? MaxPrice { get; set; }

        public Int32 Price { get; set; }

        public DateTime CreateAt { get; set; }

        public List<string>? Images { get; set; }

        public Int32 Stock { get; set; }

        public Int32 Sold { get; set; } = 0;

        public Int64 View { get; set; } = 1;

        [BsonElement("Brand_name")]
        public string? BrandName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Product_CategoriesID")]
        public List<string>? CategoryID { get; set; }

        public double? Weight { get; set; }

        public List<Speacial>? Special { get; set; }
    }
    public class Speacial
    {
        public string? k { get; set; }
        public string? v { get; set; }
    }
}
