using AutoMapper;
using CarAutomotive.Application.Dtos;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<ProductDto>> GetProductsAsync(ProductFilterDto filter)
        {
            var spec = new ProductsWithCategorySpec(
                filter.Sort,
                filter.CategoryId,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Search,
                filter.PageIndex,
                filter.PageSize);

            var products = await _unitOfWork
                .Repository<Product>()
                .GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<ProductDto>>(products);

            var countSpec = new ProductsWithFilterForCountSpec(
                filter.CategoryId,
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

            return true;
        }
    }
}
