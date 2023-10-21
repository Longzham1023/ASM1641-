using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IBookService
    {
        Task<BookResult> GetBooks(int pageNumber);
        Task<Book> GetByID(string id);
        Task CreateBook(Book aBook, IWebHostEnvironment hostingEnvironment);
        Task UpdateBook(Book aBook, string Id);
        Task RemoveBook(string Id);
        Task AddCategoryToBook(string bookId, string categoryId);
        Task<IEnumerable<Book>> SearchBook(string bookName, int pageSize, int pageNumber);
        Task<IEnumerable<Book>> GetBookByCategory(string aCategory, int pageSize, int pageNumber);
    }
}
