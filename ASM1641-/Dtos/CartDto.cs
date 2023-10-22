using System;
namespace ASM1641_.Dtos
{
    public class CartDto
    {
        public string message { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public int quantity { get; set; }
        public string imagePath { get; set; } = string.Empty;
        public decimal price { get; set; }
    }
}

