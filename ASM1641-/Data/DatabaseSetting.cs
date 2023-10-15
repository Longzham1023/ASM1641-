using System;
namespace BookStore.Data
{
	public class DatabaseSetting
	{
		public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? CategoriesCollection { get; set; }
        public string? BooksCollection { get; set; }
        public string? AuthorCollection { get; set; }
        public string? PublishersCollection { get; set; }
        public string? CustomerCollection { get; set; }
        public string? OrderCollection { get; set; }
        public string? OrderItemsCollection { get; set; }
        public string? CartCollection { get; set; }
    }
}

