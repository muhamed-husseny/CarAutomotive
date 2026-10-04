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
                                         (!minPrice.HasValue || p.BasePriceEgp >= minPrice.Value) &&
                                         (!maxPrice.HasValue || p.BasePriceEgp <= maxPrice.Value) &&
                                         (string.IsNullOrEmpty(search)|| p.Title.ToLower().Contains(search.ToLower())|| p.Brand.Name.ToLower().Contains(search.ToLower())))
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
                        AddOrderBy(p => p.BasePriceEgp);
                        break;

                    case "priceDesc":
                        AddOrderByDescending(p => p.BasePriceEgp);
                        break;

                    case "nameAsc":
                        AddOrderBy(p => p.Title);
                        break;

                    case "nameDesc":
                        AddOrderByDescending(p => p.Title);
                        break;

                    case "stockAsc":
                        AddOrderBy(p => p.StockQuantity);
                        break;

                    case "stockDesc":
                        AddOrderByDescending(p => p.StockQuantity);
                        break;
                    case "brandAsc":
                        AddOrderBy(p => p.Brand.Name);
                        break;

                    case "brandDesc":
                        AddOrderByDescending(p => p.Brand.Name);
                        break;

                    default:
                        AddOrderBy(p => p.Title);
                        break;
                }
            }
            else AddOrderBy (p => p.Id);
            ApplyPagination(pageIndex, pageSize);
        }
        public ProductsWithCategorySpec(Guid id) : base(p => p.Id == id) 
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
            AddInclude(p => p.Brand);
            AddInclude(p => p.Compatibilities);
        }
        public ProductsWithCategorySpec(int id) : base(p => false) 
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.ProductImages);
            AddInclude(p => p.Brand);
            AddInclude(p => p.Compatibilities);
        }
       
    }
}
