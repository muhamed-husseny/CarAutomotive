using CarAutomotive.Application.Dtos;
namespace CarAutomotive.Application.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
    }
}