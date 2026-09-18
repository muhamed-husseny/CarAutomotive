using CarAutomotive.Core.DTOs;

namespace CarAutomotive.Core.Interfaces
{
    public interface IProductFitmentService
    {
        Task<FitmentResultDto> CheckFitmentAsync(FitmentCheckRequestDto request);
    }
}
