using CarAutomotive.Core.Dtos;

namespace CarAutomotive.Core.Interfaces
{
    public interface IVehicleService
    {
        Task<IReadOnlyList<VehicleDto>> GetClientVehiclesAsync(Guid clientId);
        Task<VehicleDto> AddVehicleAsync(Guid clientId, CreateVehicleDto vehicleDto);
    }
}
