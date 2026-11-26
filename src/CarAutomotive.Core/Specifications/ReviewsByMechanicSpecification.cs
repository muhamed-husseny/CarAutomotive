namespace CarAutomotive.Core.Specifications
{
    public class ReviewsByMechanicSpecification : BaseSpecification<Review>
    {
        public ReviewsByMechanicSpecification(Guid mechanicId)
            : base(r => r.MechanicId == mechanicId)
        {
            
            AddInclude(r => r.User);

         
            AddOrderByDescending(r => r.CreatedAt);
        }
    }
}
