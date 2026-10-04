namespace CarAutomotive.Application.Services
{
    public interface ICompatibilityService
    {
        Task<CompatibilityDto> CreateAsync(CreateCompatibilityDto dto);

        Task<IReadOnlyList<CompatibilityDto>>
            GetByProductIdAsync(int productId);

        Task<bool> DeleteAsync(int id);
    }
}
