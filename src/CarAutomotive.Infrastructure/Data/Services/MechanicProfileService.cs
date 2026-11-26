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

        public async Task<IReadOnlyList<MechanicProfileDto>> GetAllMechanicsAsync(
            string? city = null,
            double? minRating = null,
            string? serviceName = null)
        {
            var spec = new MechanicsFilterSpec(city, minRating, serviceName);
            var mechanics = await _unitOfWork.Repository<MechanicProfile>().ListAsync(spec);
            return _mapper.Map<IReadOnlyList<MechanicProfileDto>>(mechanics);
        }

        public async Task<MechanicProfileDto> GetMechanicProfileByIdAsync(Guid id)
        {
            var spec = new MechanicsFilterSpec(id);
            var mechanic = await _unitOfWork.Repository<MechanicProfile>().GetByIdWithSpecAsync(spec);
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

            for (int i = 0; i < mechanics.Count; i++)
            {
                var distanceKm = CalculateHaversineDistanceKm(
                    searchDto.Latitude,
                    searchDto.Longitude,
                    mappedMechanics[i].Latitude,
                    mappedMechanics[i].Longitude);

                mappedMechanics[i].DistanceKm = Math.Round(distanceKm, 2);
                mappedMechanics[i].DistanceInMeters = Math.Round(distanceKm * 1000.0, 1);
            }

            return mappedMechanics;
        }

        private static double CalculateHaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
    }
}