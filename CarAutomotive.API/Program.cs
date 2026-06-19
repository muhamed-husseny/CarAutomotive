
#region Configure Service

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFileStorageService, SupabaseFileStorageService>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("Supabase"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite());
});
builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
{
    var connection = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrWhiteSpace(connection))
        throw new InvalidOperationException("Redis connection string is missing.");
    return ConnectionMultiplexer.Connect(connection);
});
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfiles>();
});

//builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(typeof(CreateMechanicProfileDtoValidator).Assembly);


var stripeSettings = builder.Configuration.GetSection("StripeSettings");


Stripe.StripeConfiguration.ApiKey = stripeSettings["SecretKey"];

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "StrictPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); 
        opt.PermitLimit = 5;                  
        opt.QueueLimit = 0;                  
    });

    options.AddFixedWindowLimiter(policyName: "GeneralPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 60;                 
        opt.QueueLimit = 2;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new { message = "Too many requests. Please try again later." }, token);
    };
});

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Cache5Mins", cacheBuilder =>
        cacheBuilder.Expire(TimeSpan.FromMinutes(5)));
});
builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

#region Configure Kestrel Middlewares
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseOutputCache();

app.UseStaticFiles();

//app.UseAuthorization();
// wrong approach, it will cause 401 Unauthorized for all endpoints, even those that don't require authentication ya fnn
//app.UseAuthentication();
app.UseAuthentication();
app.UseAuthorization(); // must be Authentication then Authorization

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{

    var dbContext = services.GetRequiredService<ApplicationDbContext>();


    await dbContext.Database.MigrateAsync();

    await StoreContextSeed.SeedAsync(dbContext);

    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await CarAutomotive.Infrastructure.Data.DataSeeds.AppIdentityDbContextSeed.SeedAdminUserAsync(userManager, roleManager);

    await CarAutomotive.Infrastructure.Data.DataSeeds.StoreContextSeed.AppIdentityDbContextSeed.SeedUsersAsync(userManager);

}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred during database migration or data seeding.");
}


#endregion

app.Run();
