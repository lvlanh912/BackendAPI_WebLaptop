using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend_WebLaptop.Model
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; } = null!;

        public Int32? Roles { get; set; } = 1!;

        public string? Fullname { get; set; } = null!;

        public string? Address { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string? WardID { get; set; }

        public Int32? Phone { get; set; }

        public string? Email { get; set; } = null!;

        public string? Profile_image { get; set; } = null!;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public Boolean? Sex { get; set; } = true;
    }
}
