using CarAutomotive.Core.Entities.Mechanic;

namespace CarAutomotive.Core.Specifications
{
    public class MechanicsFilterSpec : BaseSpecification<MechanicProfile>
    {
        public MechanicsFilterSpec(string? city = null, double? minRating = null, string? serviceName = null)
            : base(m =>
                (string.IsNullOrWhiteSpace(city) || m.Address.ToLower().Contains(city.Trim().ToLower())) &&
                (!minRating.HasValue || m.AverageRating >= minRating.Value) &&
                (string.IsNullOrWhiteSpace(serviceName) || m.Services.Any(s => s.ServiceName.ToLower().Contains(serviceName.Trim().ToLower()))))
        {
            AddInclude(m => m.Services);
            AddOrderByDescending(m => m.AverageRating);
        }

        public MechanicsFilterSpec(Guid id)
            : base(m => m.Id == id)
        {
            AddInclude(m => m.Services);
        }
    }
}
