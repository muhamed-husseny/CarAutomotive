namespace CarAutomotive.Core.DTOs.AppointmentsDto
{
    public class CreateAppointmentDto
    {
        public Guid MechanicId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
    }
}
