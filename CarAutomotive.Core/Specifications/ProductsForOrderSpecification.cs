using CarAutomotive.Core.Entities;

namespace CarAutomotive.Core.Specifications
{
    public class ProductsForOrderSpecification : BaseSpecification<Product>
    {
        public ProductsForOrderSpecification(
            IReadOnlyList<Guid> productIds)
            : base(p => productIds.Contains(p.Id))
        {
            AddInclude(p => p.ProductImages);
        }

        public ProductsForOrderSpecification(
            IReadOnlyList<int> productIds)
            : base(p => false)
        {
            AddInclude(p => p.ProductImages);
        }
    }
}