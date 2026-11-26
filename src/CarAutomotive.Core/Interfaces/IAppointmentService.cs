using CarAutomotive.Core.DTOs.AppointmentsDto;

namespace CarAutomotive.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(Guid userId, CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, Guid requestingUserId, bool isAdmin = false);
        Task<IReadOnlyList<AppointmentDto>> GetUserAppointmentsAsync(Guid userId);
        Task<IReadOnlyList<AppointmentDto>> GetMechanicAppointmentsAsync(Guid mechanicId, Guid requestingUserId, bool isAdmin = false);
        Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId, Guid requestingUserId, bool isAdmin = false);

    }
}
