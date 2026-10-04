using CarAutomotive.Core.Entities;

namespace CarAutomotive.Core.Specifications
{
    public class VehiclesByClientSpec : BaseSpecification<Vehicle>
    {
        public VehiclesByClientSpec(Guid clientId)
            : base(v => v.AppUserId == clientId)
        {
        }
    }
}
