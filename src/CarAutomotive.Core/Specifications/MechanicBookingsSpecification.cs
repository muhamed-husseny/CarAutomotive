using CarAutomotive.Core.Entities.Appointment;

namespace CarAutomotive.Core.Specifications
{
    public class MechanicBookingsSpecification : BaseSpecification<Appointment>
    {
        public MechanicBookingsSpecification(Guid mechanicId)
            : base(a => a.MechanicId == mechanicId || a.Mechanic.UserId == mechanicId)
        {
            AddInclude(a => a.User);
            AddInclude(a => a.Vehicle);
            AddInclude(a => a.Invoice);
            AddInclude("Invoice.Parts");
            AddOrderByDescending(a => a.AppointmentDate);
        }

        public MechanicBookingsSpecification(Guid mechanicId, Guid appointmentId)
            : base(a => (a.AppointmentId == appointmentId || a.Id.ToString() == appointmentId.ToString()) &&
                        (a.MechanicId == mechanicId || a.Mechanic.UserId == mechanicId))
        {
            AddInclude(a => a.User);
            AddInclude(a => a.Vehicle);
            AddInclude(a => a.Invoice);
            AddInclude("Invoice.Parts");
        }
    }
}
