namespace CarAutomotive.API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Repositories & Unit of Work
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

            // Caching, Storage & Auth
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IFileStorageService, SupabaseFileStorageService>();
            services.AddScoped<ITokenService, TokenService>();

            // Application & Domain Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICompatibilityService, CompatibilityService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IMechanicService, MechanicProfileService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IProductFitmentService, ProductFitmentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IMechanicBookingService, MechanicBookingService>();

            // AutoMapper (single registration discovering all profiles in CarAutomotive.Application)
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfiles).Assembly));

            // FluentValidation (discovers validators across both Application and Core assemblies)
            services.AddValidatorsFromAssemblies(new[]
            {
                typeof(CreateProductDtoValidator).Assembly,
                typeof(CreateMechanicProfileDtoValidator).Assembly
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = config["Token:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Token:Audience"],
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role
                };
            });

            return services;
        }
    }
}