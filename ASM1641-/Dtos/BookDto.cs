using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASM1641_.Dtos
{
	public class BookDto
	{
 
        public string Title { get; set; } = string.Empty;

   
        public string Publisher { get; set; } = string.Empty;

 
        public string PublishDate { get; set; } = string.Empty;

      
        public string AuthorId { get; set; } = string.Empty;


        public string Description { get; set; } = string.Empty;

    
        public List<string> BookCategories { get; set; } = new List<string>();

        public decimal Price { get; set; }


        public IFormFile? Image { get; set; }

        public int Quantity { get; set; }
    }
}

