using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooks();
        Task<Book> GetbyID(string id);
        Task CreateBook(Book aBook);
        Task DeleteBook(string Id);
        Task UpdateBook(Book aBook, string Id);
        Task AddCategorytoBook(string Id, string categoryId);
        Task<IEnumerable<Book>> SearchBook(string bookName);
    }
}
