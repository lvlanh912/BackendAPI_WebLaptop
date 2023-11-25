using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend_WebLaptop.Model
{
    public class Voucher
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Code { get; set; }
        [BsonElement("Min_apply")]
        public int? MinApply { get; set; }
        public int Value { get; set; }
        public bool IsValue { get; set; }
        public int MaxReduce { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Products { get; set; } = new List<string>();
        public int? Quantity { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool Active { get; set; } = false;
    }

}
