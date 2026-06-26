namespace CarAutomotive.Core.Specifications
{
    public class UserVehiclesSpecification : BaseSpecification<Vehicle>
    {
        
        public UserVehiclesSpecification(Guid userId)
            : base(v => v.AppUserId == userId)
        {
            
        }
    }
}