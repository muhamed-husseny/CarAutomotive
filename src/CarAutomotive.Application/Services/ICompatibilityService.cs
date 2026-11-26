namespace CarAutomotive.Application.Services
{
    public interface ICompatibilityService
    {
        Task<CompatibilityDto> CreateAsync(CreateCompatibilityDto dto);

        Task<IReadOnlyList<CompatibilityDto>>
            GetByProductIdAsync(Guid productId);

        Task<bool> DeleteAsync(int id);
    }
}
