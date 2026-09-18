using CarAutomotive.Core.DTOs;

namespace CarAutomotive.Core.Interfaces
{
    public interface IMechanicBookingService
    {
        Task<IReadOnlyList<MechanicBookingDto>> GetMechanicBookingsAsync(Guid mechanicId);
        Task<bool> UpdateBookingStatusAsync(Guid mechanicId, Guid appointmentId, string status);
        Task<bool> GenerateInvoiceAsync(Guid mechanicId, Guid appointmentId, CreateInvoiceDto dto);
    }
}
