namespace CarAutomotive.Core.Specifications
{
    public class AppointmentByMechanicSpecification : BaseSpecification<Appointment>
    {
        public AppointmentByMechanicSpecification(Guid mechanicId)
            : base(a => a.MechanicId == mechanicId)
        {
            AddInclude(a => a.Mechanic);

            AddOrderByDescending(a => a.AppointmentDate);
        }

    }
}
