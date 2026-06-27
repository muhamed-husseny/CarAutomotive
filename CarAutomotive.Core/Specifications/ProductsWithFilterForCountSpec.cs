using CarAutomotive.Core.Entities;
namespace CarAutomotive.Core.Specifications
{
    public class ProductsWithFilterForCountSpec : BaseSpecification<Product>
    {
        public ProductsWithFilterForCountSpec(
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice,
            string? search)
            : base(p =>
                (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                (!brandId.HasValue || p.BrandId == brandId.Value) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                (string.IsNullOrEmpty(search)|| p.Name.ToLower().Contains(search.ToLower())|| p.Brand.Name.ToLower().Contains(search.ToLower()))
            )
        {
        }
    }
}