using Microsoft.AspNetCore.Http;
namespace CarAutomotive.Application.Services
{
    public interface IProductService 
    {
        Task<Pagination<ProductDto>> GetProductsAsync(ProductFilterDto filter);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateProductAsync(int id , UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task<string?> UploadProductImageAsync(int productId, IFormFile file);
        Task<bool> DeleteProductImageAsync(int productId, int imageId);
    }
}
