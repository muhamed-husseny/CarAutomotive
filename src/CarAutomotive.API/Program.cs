using Microsoft.AspNetCore.HttpOverrides;
#region Configure Service

using Resend;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddCors(opts => opts.AddPolicy("DevPolicy",
    b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

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
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CarAutomotive API V1");
});

app.UseCors("DevPolicy");

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

#endregion

app.Run();


public partial class Program { }
