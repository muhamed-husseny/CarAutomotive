using CarAutomotive.Core.Specifications;

public class ProductsForOrderSpecification : BaseSpecification<Product>
{
    public ProductsForOrderSpecification(
        IReadOnlyList<int> productIds)
        : base(p => productIds.Contains(p.Id))
    {
        AddInclude(p => p.ProductImages);
    }
}