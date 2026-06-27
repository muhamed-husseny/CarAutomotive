using CarAutomotive.Core.Entities;
namespace CarAutomotive.Core.Specifications
{
    public class ProductsWithCategorySpec : BaseSpecification<Product>
    {
        public ProductsWithCategorySpec(string? sort,
                                        int? categoryId,
                                        int? brandId,
                                        decimal? minPrice,
                                        decimal? maxPrice,
                                        string? search,
                                        int pageIndex,
                                        int pageSize) 
                                        : base(p =>
                                         (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                                         (!brandId.HasValue || p.BrandId == brandId.Value)&&
                                         (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                                         (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                                         (string.IsNullOrEmpty(search)|| p.Name.ToLower().Contains(search.ToLower())|| p.Brand.Name.ToLower().Contains(search.ToLower())))
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
            AddInclude(p => p.Brand);
            AddInclude(p => p.Compatibilities);
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
                    case "brandAsc":
                        AddOrderBy(p => p.Brand.Name);
                        break;

                    case "brandDesc":
                        AddOrderByDescending(p => p.Brand.Name);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else AddOrderBy (p => p.Id);
            ApplyPagination(pageIndex, pageSize);
        }
        public ProductsWithCategorySpec(int id) : base(p => p.Id == id) 
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
            AddInclude(p => p.Brand);
            AddInclude(p => p.Compatibilities);
        }
       
    }
}
