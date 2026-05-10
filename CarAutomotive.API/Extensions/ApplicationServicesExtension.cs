namespace CarAutomotive.API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IMechanicService, MechanicProfileService>();

            services.AddAutoMapper(config => config.AddMaps(typeof(MechanicMappingProfile).Assembly)); services.AddAuthentication(options =>
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
                    ValidateLifetime = true
                };
            });

            
            return services;
        }
    }
}