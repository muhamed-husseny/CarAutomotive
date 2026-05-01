namespace CarAutomotive.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.ProductImages, o => o.MapFrom(s => s.ProductImages.Select(pi => pi.ImageUrl).ToList()));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.ProductImages,o => o.MapFrom(s => s.ImageUrls.Select(url => new ProductImage
                {
                     ImageUrl = url
                })));
            CreateMap<UpdateProductDto, Product>()
                .ForMember(d => d.ProductImages, o => o.MapFrom(s => s.ImageUrls.Select(url => new ProductImage
                {
                     ImageUrl = url
                })));
        }
    }
}
