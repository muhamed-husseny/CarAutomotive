namespace CarAutomotive.Core.Specifications
{
    public class CategoryWithProductsSpecification : BaseSpecification<Category>
    {
        public CategoryWithProductsSpecification(int id) : base(c => c.Id == id)
        {
            AddInclude(c => c.Products);
        }
    }


}
