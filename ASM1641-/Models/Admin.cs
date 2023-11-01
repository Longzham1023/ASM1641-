using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ASM1641_.Models
{
	public class Admin
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Role")]
        [BsonRequired]
        public string Role { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}

