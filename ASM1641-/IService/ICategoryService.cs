using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category> GetByID(string id);
        Task CreateCategory(Category category);
        Task DeleteCategory(string Id);
        Task UpdateCategory(Category category, string Id);
    }
}
