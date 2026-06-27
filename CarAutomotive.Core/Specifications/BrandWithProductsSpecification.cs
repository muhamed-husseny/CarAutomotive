namespace CarAutomotive.Core.Specifications
{
    public class BrandWithProductsSpecification
    : BaseSpecification<Brand>
    {
        public BrandWithProductsSpecification(int id)
            : base(b => b.Id == id)
        {
            AddInclude(b => b.Products);
        }
    }
}
