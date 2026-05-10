namespace CarAutomotive.Core.Entities.Appointment
{
    public class Appointment : BaseEntity
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        public Guid MechanicId { get; set; }
        public MechanicProfile Mechanic { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    }
}
