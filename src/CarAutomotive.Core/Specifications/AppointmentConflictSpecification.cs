namespace CarAutomotive.Core.Specifications
{
    public class AppointmentConflictSpecification : BaseSpecification<Appointment>
    {
        public AppointmentConflictSpecification(Guid mechanicId, DateTime appointmentDate)
            : base(a => a.MechanicId == mechanicId &&
                        a.AppointmentDate == appointmentDate &&
                        a.Status != AppointmentStatus.Cancelled)
        {
        }
    }
}