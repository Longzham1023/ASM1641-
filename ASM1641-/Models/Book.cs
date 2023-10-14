using System;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASM1641_.Models
{
	public class Book
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Title")]
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        [BsonElement("Publisher")]
        [BsonRequired]
        public string Publisher { get; set; } = string.Empty;

        [BsonElement("PublishDate")]
        [BsonRequired]
        public string PublishDate { get; set; } = string.Empty;

        [BsonElement("Genre")]
        [BsonRequired]
        public string Genre { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("AuthorId")]
        [BsonRequired]
        public string AuthorId { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BookCategories")]
        [BsonRequired]
        public List<string> BookCategories { get; set; } = new List<string>();

        [BsonElement("Price")]
        //[BsonRequired]
        public decimal Price { get; set; }

        [BsonIgnore]
        public IFormFile? Image { get; set; }

        [BsonElement("ImagePath")]
        public string ImagePath { get; set; } = string.Empty;
    }
}

