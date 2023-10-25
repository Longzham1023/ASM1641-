using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IBookService
    {
        Task<BookResult> GetBooks(int pageNumber);
        Task<Book> GetByID(string id);
        Task CreateBook(Book aBook, IWebHostEnvironment hostingEnvironment);
        Task UpdateBook(BookDto updatedBook, string bookId, IWebHostEnvironment hostingEnvironment);
        Task RemoveBook(string Id);
        Task AddCategoryToBook(string bookId, string categoryId);
        Task<BookResult> SearchBook(string bookName, int pageNumber);
        Task<BookResult> GetBookByCategory(string aCategory, int pageNumber);
    }
}
