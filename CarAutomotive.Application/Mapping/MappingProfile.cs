using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Application.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName,
                    o => o.MapFrom(s => s.Category.Name))

                .ForMember(d => d.BrandName,
                    o => o.MapFrom(s => s.Brand.Name))

                .ForMember(d => d.ProductImages,
                    o => o.MapFrom(s => s.ProductImages.Select(pi => pi.ImageUrl).ToList()));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.ProductImages,
                    o => o.MapFrom(s => s.ImageUrls.Select(url => new ProductImage
                    {
                        ImageUrl = url
                    })));

            CreateMap<UpdateProductDto, Product>()
                .ForMember(d => d.ProductImages,
                    o => o.MapFrom(s => s.ImageUrls.Select(url => new ProductImage
                    {
                        ImageUrl = url
                    })));
            CreateMap<Category, CategoryDto>();
            CreateMap<Brand, BrandDto>();
            CreateMap<ShoppingCart, CartDto>();
            CreateMap<CartItem, CartItemDto>();
            CreateMap<ShippingAddress, ShippingAddressDto>();

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(
                    d => d.Status,
                    o => o.MapFrom(s => s.Status.ToString()));

        }
    }
}