using CarAutomotive.Core.Specifications;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace CarAutomotive.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IResponseCacheService _cacheService;
        private readonly IGenericRepository<Vehicle> _vehicleRepo;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService, IResponseCacheService cacheService, IGenericRepository<Vehicle> vehicleRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _cacheService = cacheService;
            _vehicleRepo = vehicleRepo;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var spec = new ProductsWithCategorySpec(id);

            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdWithSpecAsync(spec);

            if (product == null)
                return null;

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            product.CreatedDate = DateTime.UtcNow;

            _unitOfWork.Repository<Product>().Add(product);

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveCacheResponseAsync("/api/products");
            var spec = new ProductsWithCategorySpec(product.Id);

            var createdProduct = await _unitOfWork
                .Repository<Product>()
                .GetByIdWithSpecAsync(spec);

            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var spec = new ProductsWithCategorySpec(id);

            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdWithSpecAsync(spec);

            if (product == null)
                return null;

            _mapper.Map(dto, product);

            _unitOfWork.Repository<Product>().Update(product);

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveCacheResponseAsync("/api/products");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdAsync(id);

            if (product == null)
                return false;

            _unitOfWork.Repository<Product>().Delete(product);

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveCacheResponseAsync("/api/products");
            return true;
        }

        public async Task<string?> UploadProductImageAsync(int productId, IFormFile file)
        {
            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdAsync(productId);

            if (product == null)
                return null;

            var imageUrl = await _fileStorageService.UploadFileAsync(file, "products");

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl
            };

            _unitOfWork.Repository<ProductImage>().Add(productImage);

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveCacheResponseAsync("/api/products");
            return imageUrl;
        }

        public async Task<bool> DeleteProductImageAsync(int productId, int imageId)
        {
            var productImage = await _unitOfWork
               .Repository<ProductImage>()
               .GetByIdAsync(imageId);

            if (productImage?.ImageUrl == null || productImage.ProductId != productId)
                return false;

            await _fileStorageService.DeleteFileAsync(productImage.ImageUrl);
            _unitOfWork.Repository<ProductImage>().Delete(productImage);
            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveCacheResponseAsync("/api/products");

            return true;
        }
        public async Task<Pagination<ProductDto>> GetProductsAsync(
    ProductFilterDto filter,
    Guid? userId = null)
        {
            var spec = new ProductsWithCategorySpec(
                filter.Sort,
                filter.CategoryId,
                filter.BrandId,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Search,
                filter.PageIndex,
                filter.PageSize);

            var products = await _unitOfWork
                .Repository<Product>()
                .GetAllWithSpecAsync(spec);

            var data = _mapper.Map<List<ProductDto>>(products);


            if (userId.HasValue)
            {
                var vehicles = await _unitOfWork
                    .Repository<Vehicle>()
                    .ListAsync(
                        new UserVehiclesSpecification(
                            userId.Value));


                foreach (var product in data)
                {
                    product.IsCompatible = vehicles.Any(v =>

                        product.Compatibilities.Any(c =>

                            string.Equals(
                                c.Make,
                                v.Make,
                                StringComparison.OrdinalIgnoreCase)

                            &&

                            string.Equals(
                                c.Model,
                                v.Model,
                                StringComparison.OrdinalIgnoreCase)

                            &&

                            c.Year == v.Year
                        ));
                }
            }


            var countSpec =
                new ProductsWithFilterForCountSpec(
                    filter.CategoryId,
                    filter.BrandId,
                    filter.MinPrice,
                    filter.MaxPrice,
                    filter.Search);


            var count = await _unitOfWork
                .Repository<Product>()
                .CountAsync(countSpec);


            return new Pagination<ProductDto>(
                filter.PageIndex,
                filter.PageSize,
                count,
                data);
        }
    }
}