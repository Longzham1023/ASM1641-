using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BookStore.Models
{
	public class CartItems
	{
		[BsonElement("BookId")]
		[BsonRequired]
		public string BookId { get; set; } = string.Empty;

		[BsonElement("Quantity")]
		[BsonRequired]
		public int Quantity { get; set; }

		[BsonElement("Price")]
		public decimal Price { get; set; }
	}
}

