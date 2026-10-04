using CarAutomotive.Core.Errors;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<BrandDto>> GetBrandsAsync()
        {
            var brands = (await _unitOfWork
                 .Repository<Brand>()
                 .GetAllAsync())
                 .OrderBy(b => b.Name)
                 .ToList();

            return _mapper.Map<IReadOnlyList<BrandDto>>(brands);
        }
        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _unitOfWork
                .Repository<Brand>()
                .GetByIdAsync(id);

            return brand is null
                ? null
                : _mapper.Map<BrandDto>(brand);
        }
        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
        {
            var brand = new Brand
            {
                Name = dto.Name
            };

            _unitOfWork.Repository<Brand>()
                .Add(brand);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BrandDto>(brand);
        }
        public async Task<BrandDto?> UpdateBrandAsync( int id,UpdateBrandDto dto)
        {
            var brand = await _unitOfWork
                .Repository<Brand>()
                .GetByIdAsync(id);

            if (brand is null)
                return null;

            brand.Name = dto.Name;

            _unitOfWork.Repository<Brand>()
                .Update(brand);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BrandDto>(brand);
        }
        public async Task<bool> DeleteBrandAsync(int id)
        {
            var spec = new BrandWithProductsSpecification(id);

            var brand = await _unitOfWork
                .Repository<Brand>()
                .GetEntityWithSpec(spec);

            if (brand is null)
                return false;

            if (brand.Products.Any())
            {
                throw new BadRequestException(
                    "Cannot delete brand because it contains products.");
            }

            _unitOfWork.Repository<Brand>()
                .Delete(brand);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
