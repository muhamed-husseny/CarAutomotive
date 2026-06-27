namespace CarAutomotive.Application.Services
{
    public class CompatibilityService : ICompatibilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompatibilityService(
                IUnitOfWork unitOfWork,
                IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CompatibilityDto> CreateAsync(CreateCompatibilityDto dto)
        {
            var compatibility = _mapper.Map<Compatibility>(dto);

            _unitOfWork.Repository<Compatibility>()
                       .Add(compatibility);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CompatibilityDto>(compatibility);
        }

        public async Task<IReadOnlyList<CompatibilityDto>>
        GetByProductIdAsync(int productId)
        {

            var items = await _unitOfWork
     .Repository<Compatibility>()
     .GetAllAsync();

            var result = items
                .Where(x => x.ProductId == productId)
                .ToList();

            return _mapper.Map<IReadOnlyList<CompatibilityDto>>(result);
        }
        public async Task<bool> DeleteAsync(int id)
        {

            var compatibility = await _unitOfWork
                .Repository<Compatibility>()
                .GetByIdAsync(id);


            if (compatibility is null)
                return false;

            _unitOfWork
                .Repository<Compatibility>()
                .Delete(compatibility);

            await _unitOfWork.CompleteAsync();

            return true;

        }
    }
}
