using Microsoft.AspNetCore.Http;
namespace CarAutomotive.Application.Services
{
    public interface IProductService 
    {
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateProductAsync(Guid id , UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<string?> UploadProductImageAsync(Guid productId, IFormFile file);
        Task<bool> DeleteProductImageAsync(Guid productId, int imageId);
        Task<Pagination<ProductDto>>
GetProductsAsync(
    ProductFilterDto filter,
    Guid? userId = null);

    }
}
