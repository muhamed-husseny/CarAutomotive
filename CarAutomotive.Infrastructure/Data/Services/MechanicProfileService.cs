namespace CarAutomotive.Infrastructure.Services 
{
    public class MechanicProfileService : IMechanicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MechanicProfileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MechanicProfileDto> CreateMechanicProfileAsync(CreateMechanicProfileDto dto)
        {
            var mechanic = _mapper.Map<MechanicProfile>(dto);
            mechanic.Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 };

            _unitOfWork.Repository<MechanicProfile>().Add(mechanic);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MechanicProfileDto>(mechanic);
        }

        public async Task<IReadOnlyList<MechanicProfileDto>> GetAllMechanicsAsync()
        {
            
            var mechanics = await _unitOfWork.Repository<MechanicProfile>().GetAll();
            return _mapper.Map<IReadOnlyList<MechanicProfileDto>>(mechanics);
        }

        public async Task<MechanicProfileDto> GetMechanicProfileByIdAsync(Guid id)
        {
            var mechanic = await _unitOfWork.Repository<MechanicProfile>().GetById(id);
            if (mechanic == null) return null;

            return _mapper.Map<MechanicProfileDto>(mechanic);
        }

        public async Task<IReadOnlyList<MechanicProfileDto>> GetMechanicsByCityAsync(string city)
        {
            
            var spec = new MechanicsByCitySpec(city);
            var mechanics = await _unitOfWork.Repository<MechanicProfile>().ListAsync(spec);

            return _mapper.Map<IReadOnlyList<MechanicProfileDto>>(mechanics);
        }

        public async Task<IReadOnlyList<NearbyMechanicDto>> SearchNearbyMechanicsAsync(MechanicSearchDto searchDto)
        {
            var spec = new MechanicsByLocationSpec(searchDto.Latitude, searchDto.Longitude, searchDto.RadiusInKilometers);
            var mechanics = await _unitOfWork.Repository<MechanicProfile>().ListAsync(spec);

            var mappedMechanics = _mapper.Map<IReadOnlyList<NearbyMechanicDto>>(mechanics);
            var userLocation = new Point(searchDto.Longitude, searchDto.Latitude) { SRID = 4326 };

            for (int i = 0; i < mechanics.Count; i++)
            {
                mappedMechanics[i].DistanceInMeters = mechanics[i].Location.Distance(userLocation);
            }

            return mappedMechanics;
        }
    }
}