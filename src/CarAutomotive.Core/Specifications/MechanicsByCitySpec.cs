namespace CarAutomotive.Core.Specifications
{
    public class MechanicsByCitySpec : BaseSpecification<MechanicProfile>
    {
        public MechanicsByCitySpec(string city)
            : base(m => m.Address.ToLower().Contains(city.ToLower()))
        {
            AddInclude(m => m.Services);
            AddOrderByDescending(m => m.AverageRating);
        }
    }
}
