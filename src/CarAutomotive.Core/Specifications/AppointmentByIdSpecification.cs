using CarAutomotive.Core.Entities.Appointment;

namespace CarAutomotive.Core.Specifications
{
    public class AppointmentByIdSpecification : BaseSpecification<Appointment>
    {
        public AppointmentByIdSpecification(Guid appointmentId)
            : base(a => a.AppointmentId == appointmentId)
        {
            AddInclude(a => a.Mechanic);
            AddInclude(a => a.User);
        }
    }
}
