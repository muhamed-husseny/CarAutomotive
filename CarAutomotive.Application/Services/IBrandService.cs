namespace CarAutomotive.Application.Services
{
    public interface IBrandService
    {
        Task<IReadOnlyList<BrandDto>> GetBrandsAsync();

        Task<BrandDto?> GetBrandByIdAsync(int id);

        Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);

        Task<BrandDto?> UpdateBrandAsync(
            int id,
            UpdateBrandDto dto);

        Task<bool> DeleteBrandAsync(int id);
    }
}
