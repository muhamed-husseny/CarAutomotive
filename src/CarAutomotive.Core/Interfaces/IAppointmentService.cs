using CarAutomotive.Core.DTOs.AppointmentsDto;

namespace CarAutomotive.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(Guid userId, CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, Guid requestingUserId);
        Task<IReadOnlyList<AppointmentDto>> GetUserAppointmentsAsync(Guid userId);
        Task<IReadOnlyList<AppointmentDto>> GetMechanicAppointmentsAsync(Guid mechanicId);
        Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId);

    }
}
