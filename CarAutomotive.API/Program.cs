#region Configure Service

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite());
});

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();

    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await CarAutomotive.Infrastructure.Data.DataSeeds.AppIdentityDbContextSeed.SeedAdminUserAsync(userManager, roleManager);

    await CarAutomotive.Infrastructure.Data.DataSeeds.StoreContextSeed.AppIdentityDbContextSeed.SeedUsersAsync(userManager);

    await CarAutomotive.Infrastructure.Data.DataSeeds.MechanicContextSeed.SeedAsync(dbContext, userManager);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred during database migration or data seeding.");
}


#endregion

app.Run();
