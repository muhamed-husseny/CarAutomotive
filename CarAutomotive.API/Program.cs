
#region Configure Service
using CarAutomotive.Infrastructure.Data.DataSeeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite());
});
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfiles>();
});

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddApplicationServices(builder.Configuration);

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

app.UseStaticFiles();

app.UseAuthorization();

app.UseAuthentication();

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
    await CarAutomotive.Infrastructure.Data.DataSeeds.StoreContextSeed.AppIdentityDbContextSeed.SeedUsersAsync(userManager);

}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred during database migration or data seeding.");
}
#endregion

app.Run();
