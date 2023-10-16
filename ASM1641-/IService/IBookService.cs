using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllCategories(int pageSize, int pageNumber);
        Task<Book> GetbyID(string id);
        Task CreateCategory(Category category);
        Task UpdateCategory(string name, string categoryId);
        Task RemoveCategory(string Id);
    }
}
