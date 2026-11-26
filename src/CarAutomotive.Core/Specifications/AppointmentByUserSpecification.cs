namespace CarAutomotive.Core.Specifications
{
    public class AppointmentByUserSpecification : BaseSpecification<Appointment>
    {
        public AppointmentByUserSpecification(Guid userId)
            : base(a => a.UserId == userId)
        {
            AddInclude(a => a.Mechanic);
            AddOrderByDescending(a => a.AppointmentDate);
        }
    }
}
