using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories(int pageSize, int pageNumber);
        Task<Category> GetByID(string id);
        Task CreateCategory(Category category);
        Task RemoveCategory(string Id);
        Task UpdateCategory(string name, string categoryId);
    }
}
