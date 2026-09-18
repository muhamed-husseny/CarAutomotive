using CarAutomotive.Core.Dtos;
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
        o => o.MapFrom(s =>
            s.ProductImages.Select(pi => pi.ImageUrl).ToList()))

    .ForMember(d => d.Compatibilities,
        o => o.MapFrom(s => s.Compatibilities));

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
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => Math.Abs(s.ProductId.GetHashCode())));
            CreateMap<ShippingAddress, ShippingAddressDto>();
            CreateMap<Compatibility, CompatibilityDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => Math.Abs(s.ProductId.GetHashCode())));

            CreateMap<CreateCompatibilityDto, Compatibility>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => Guid.Empty));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => Math.Abs(s.ProductId.GetHashCode())));

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(
                    d => d.Status,
                    o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.Images != null && src.Images.Count > 0
                            ? src.Images
                            : (!string.IsNullOrEmpty(src.ImageUrl) ? new List<string> { src.ImageUrl } : new List<string>())));

            CreateMap<CreateVehicleDto, Vehicle>()
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.Images != null ? src.Images.FirstOrDefault() : null))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images ?? new List<string>()));
        }
    }
}