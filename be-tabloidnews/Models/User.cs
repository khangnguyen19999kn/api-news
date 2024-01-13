using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace be_tabloidnews.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
        [BsonElement("displayName")]
        public string displayName { get; set; }
        [BsonElement("roleId")]
        public string roleId { get; set; }
    }
}
