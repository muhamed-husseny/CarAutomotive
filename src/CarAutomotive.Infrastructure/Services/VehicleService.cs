using AutoMapper;
using CarAutomotive.Core.Dtos;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Infrastructure.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<VehicleDto>> GetClientVehiclesAsync(Guid clientId)
        {
            var spec = new VehiclesByClientSpec(clientId);
            var vehicles = await _unitOfWork.Repository<Vehicle>().ListAsync(spec);
            return _mapper.Map<IReadOnlyList<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> AddVehicleAsync(Guid clientId, CreateVehicleDto vehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            vehicle.AppUserId = clientId;
            vehicle.CreatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Vehicle>().Add(vehicle);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<VehicleDto>(vehicle);
        }
    }
}
