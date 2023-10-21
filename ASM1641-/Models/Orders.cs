using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASM1641_.Models
{
    public class Orders
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string customerId { get; set; } = string.Empty;

        [BsonElement("OrderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("OrderItems")]
        public List<CartItems>? OrderItems { get; set; }

        [BsonElement("Address")]
        public string address { get; set; } = string.Empty;

        [BsonElement("Phone")]
        public string phoneNumber { get; set; } = string.Empty;
    }
}
