using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStore.Models
{
	public class Customer
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("FirstName")]
        [BsonRequired]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("LastName")]
        [BsonRequired]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("Address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("Phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("CartUser")]
        public List<CartItems>? CartItems { get; set; }
    }
}

