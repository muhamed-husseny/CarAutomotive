using Microsoft.AspNetCore.HttpOverrides;
#region Configure Service

using Resend;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var configuredOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .ToArray() ?? Array.Empty<string>();

if (configuredOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    configuredOrigins = new[]
    {
        "http://localhost:3000",
        "https://localhost:3000",
        "http://localhost:5173"
    };
}

builder.Services.AddCors(opts => opts.AddPolicy("CorsPolicy", policy =>
{
    if (configuredOrigins.Length > 0)
    {
        policy.WithOrigins(configuredOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    }
}));

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

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
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("Supabase"));

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

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddApplicationServices(builder.Configuration);


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
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});
app.UseMiddleware<ExceptionMiddleware>();

#region Configure Kestrel Middlewares
// Configure the HTTP request pipeline.
var swaggerEnabled = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CarAutomotive API V1");
    });
}

app.UseCors("CorsPolicy");

//app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseOutputCache();

app.UseStaticFiles();

//app.UseAuthorization();
// wrong approach, it will cause 401 Unauthorized for all endpoints, even those that don't require authentication ya fnn
//app.UseAuthentication();
app.UseAuthentication();
app.UseAuthorization(); // must be Authentication then Authorization

app.MapControllers();

app.MapHealthChecks("/health");

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var shouldSeedDemo = builder.Configuration.GetValue<bool>("SeedDemoData")
    || args.Any(a => string.Equals(a, "--seed-demo", StringComparison.OrdinalIgnoreCase));

if (shouldSeedDemo)
{
    await using var seedScope = app.Services.CreateAsyncScope();
    var seedServices = seedScope.ServiceProvider;
    var logger = seedServices.GetRequiredService<ILoggerFactory>().CreateLogger("DemoDataSeeder");
    try
    {
        var dbContext = seedServices.GetRequiredService<ApplicationDbContext>();
        var userManager = seedServices.GetRequiredService<UserManager<AppUser>>();
        var roleManager = seedServices.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var demoPassword = builder.Configuration["DemoSeed:DefaultPassword"];

        await DemoDataSeeder.SeedAsync(dbContext, userManager, roleManager, demoPassword);
        logger.LogInformation("Demo data seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding demo data.");
        throw;
    }
}

#endregion

app.Run();


public partial class Program { }
