using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthors();
        Task<Author> GetByID(string id);
        Task CreateAuthor(Author anAuthor);
        Task DeleteAuthor(Author anAuthor);
        Task UpdateAuthor(Author anAuthor); 
        Task <IEnumerable<Book>> GetAllBookByAuthor(string authorID);
        Task AddBookAuthor(string BookId, string authorID);
    }
}
