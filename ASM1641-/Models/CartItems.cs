﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ASM1641_.Models
{
	public class CartItems
	{

        [BsonElement("BookId")]
		[BsonRequired]
		public string BookId { get; set; } = string.Empty;

		[BsonElement("BookName")]
		public string BookName { get; set; } = string.Empty;

		[BsonElement("Quantity")]
		[BsonRequired]
		public int Quantity { get; set; }

		[BsonElement("Price")]
		public decimal Price { get; set; }

		[BsonElement("imagePath")]
		public string imagePath { get; set; } = string.Empty;

    }
}

