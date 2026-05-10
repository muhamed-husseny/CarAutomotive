
namespace CarAutomotive.Core.Interfaces
{
    public interface IMechanicService
    {
        
        Task<MechanicProfileDto> CreateMechanicProfileAsync(CreateMechanicProfileDto dto);

        
        Task<MechanicProfileDto> GetMechanicProfileByIdAsync(Guid id);

        
        Task<IReadOnlyList<NearbyMechanicDto>> SearchNearbyMechanicsAsync(MechanicSearchDto searchDto);

        Task<IReadOnlyList<MechanicProfileDto>> GetAllMechanicsAsync();

        Task<IReadOnlyList<MechanicProfileDto>> GetMechanicsByCityAsync(string city);
    }
}