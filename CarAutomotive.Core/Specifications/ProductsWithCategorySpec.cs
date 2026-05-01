using CarAutomotive.Core.Entities;
namespace CarAutomotive.Core.Specifications
{
    public class ProductsWithCategorySpec : BaseSpecification<Product>
    {
        public ProductsWithCategorySpec(string? sort,
                                        int? categoryId,
                                        decimal? minPrice,
                                        decimal? maxPrice,
                                        string? search,
                                        int pageIndex,
                                        int pageSize) 
                                        : base(p =>
                                         (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                                         (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                                         (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                                         (string.IsNullOrEmpty(search) || p.Name.ToLower().Contains(search.ToLower()))) 
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;

                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;

                    case "nameAsc":
                        AddOrderBy(p => p.Name);
                        break;

                    case "nameDesc":
                        AddOrderByDescending(p => p.Name);
                        break;

                    case "stockAsc":
                        AddOrderBy(p => p.StockCount);
                        break;

                    case "stockDesc":
                        AddOrderByDescending(p => p.StockCount);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else AddOrderBy (p => p.Name);
            ApplyPagination(pageIndex, pageSize);
        }
        public ProductsWithCategorySpec(int id) : base(p => p.Id == id) 
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
        }
       
    }
}
