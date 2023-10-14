using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStore.Models
{
	public class Author
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

        [BsonElement("Birthdate")]
        public string Birthdate { get; set; } = string.Empty;

        [BsonElement("Biography")]
        public string Biography { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BookIDs")]
        public List<string>? BookIds { get; set; } 
    }
}

