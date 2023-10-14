using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASM1641_.Models
{
	public class Category
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; } = string.Empty;

		[BsonElement("Name")]
		public string Name { get; set; } = string.Empty;
	}
}

