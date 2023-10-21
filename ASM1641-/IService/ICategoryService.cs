using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface ICategoryService
    {
        Task<CateResult>  GetAllCategories( int pageNumber);
        Task<Category> GetByID(string id);
        Task CreateCategory(Category category);
        Task RemoveCategory(string Id);
        Task UpdateCategory(string name, string categoryId);
    }
}
