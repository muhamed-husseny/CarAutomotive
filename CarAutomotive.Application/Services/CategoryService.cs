using CarAutomotive.Core.Errors;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _unitOfWork.Repository<Category>().Add(category);

            await _unitOfWork.CompleteAsync();
           
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var spec = new CategoryWithProductsSpecification(id);

            var category = await _unitOfWork
                .Repository<Category>()
                .GetEntityWithSpec(spec);

            if (category == null)
                return false;

            var hasProducts = category.Products.Any();

            if (hasProducts)
            {
                throw new BadRequestException(
                    "Cannot delete category because it contains products.");
            }
            _unitOfWork.Repository<Category>().Delete(category);

            await _unitOfWork.CompleteAsync();
            
            return true;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork
                .Repository<Category>()
                .GetAllAsync();

            return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork
                .Repository<Category>()
                .GetByIdAsync(id);

            if (category == null)
                return null;

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork
                .Repository<Category>()
                .GetByIdAsync(id);

            if (category == null)
                return null;

            category.Name = dto.Name;
            category.Description = dto.Description;

            _unitOfWork.Repository<Category>().Update(category);

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}