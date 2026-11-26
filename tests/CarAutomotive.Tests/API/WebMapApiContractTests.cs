using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CarAutomotive.Application.Services;
using CarAutomotive.Core.Entities.Mechanic;
using CarAutomotive.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NetTopologySuite.Geometries;
using StackExchange.Redis;

namespace CarAutomotive.Tests.API
{
    public class WebMapApiContractTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string TestJwtKey = "TestSuperSecretJwtSigningKeyForUnitTestsOnly123456!";
        private const string TestJwtIssuer = "CarAutomotive.Test";
        private const string TestJwtAudience = "CarAutomotive.TestClient";

        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _databaseName = "WebMapTestDatabase_" + Guid.NewGuid();

        public WebMapApiContractTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=car_automotive_test;Username=test;Password=test",
                        ["ConnectionStrings:Redis"] = "localhost:6379",
                        ["Token:Key"] = TestJwtKey,
                        ["Token:Issuer"] = TestJwtIssuer,
                        ["Token:Audience"] = TestJwtAudience,
                        ["StripeSettings:SecretKey"] = "sk_test_dummy_key_for_tests",
                        ["Swagger:Enabled"] = "true"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var dbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (dbDescriptor != null) services.Remove(dbDescriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(_databaseName));

                    var redisDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IConnectionMultiplexer));
                    if (redisDescriptor != null) services.Remove(redisDescriptor);

                    services.AddSingleton(new Mock<IConnectionMultiplexer>().Object);

                    var cacheDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IResponseCacheService));
                    if (cacheDescriptor != null) services.Remove(cacheDescriptor);

                    var mockCache = new Mock<IResponseCacheService>();
                    mockCache.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>()))
                        .ReturnsAsync((string?)null);
                    services.AddSingleton(mockCache.Object);
                });
            });
        }

        private HttpClient CreateAuthenticatedClient(Guid userId, string role)
        {
            var client = _factory.CreateClient();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, "user@test.local"),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = TestJwtIssuer,
                Audience = TestJwtAudience
            };

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityTokenHandler().CreateToken(tokenDescriptor));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task GetMechanics_WithMapFilters_ShouldReturnNumericCoordinatesAndServices()
        {
            var maadiId = Guid.NewGuid();
            var gizaId = Guid.NewGuid();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.MechanicProfiles.AddRange(
                    new MechanicProfile
                    {
                        Id = maadiId,
                        UserId = Guid.NewGuid(),
                        Name = "Maadi Precision Auto",
                        PhoneNumber = "01012345678",
                        Address = "Road 9, Maadi, Cairo",
                        Location = new Point(31.2597, 29.9602) { SRID = 4326 },
                        AverageRating = 4.8,
                        TotalReviews = 42,
                        YearsOfExperience = 10,
                        IsAvailable = true,
                        Services = new List<MechanicService>
                        {
                            new() { ServiceName = "Brake Pad Replacement", Description = "Front and rear", EstimatedPrice = 600m }
                        }
                    },
                    new MechanicProfile
                    {
                        Id = gizaId,
                        UserId = Guid.NewGuid(),
                        Name = "Dokki Quick Service",
                        PhoneNumber = "01098765432",
                        Address = "Tahrir St, Dokki, Giza",
                        Location = new Point(31.2089, 30.0385) { SRID = 4326 },
                        AverageRating = 3.9,
                        TotalReviews = 12,
                        YearsOfExperience = 4,
                        IsAvailable = true,
                        Services = new List<MechanicService>
                        {
                            new() { ServiceName = "Oil Change", Description = "Synthetic oil", EstimatedPrice = 350m }
                        }
                    });
                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();

            var filteredResponse = await client.GetAsync("/api/v1/mechanics?city=Maadi&minRating=4.5&serviceName=Brake");
            Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);

            var filtered = await filteredResponse.Content.ReadFromJsonAsync<List<MechanicProfileDto>>();
            Assert.NotNull(filtered);
            Assert.Single(filtered);
            Assert.Equal(maadiId, filtered[0].Id);
            Assert.Equal(29.9602, filtered[0].Latitude, precision: 4);
            Assert.Equal(31.2597, filtered[0].Longitude, precision: 4);
            Assert.Single(filtered[0].Services);
            Assert.Equal("Brake Pad Replacement", filtered[0].Services[0].ServiceName);

            var cityResponse = await client.GetAsync("/api/v1/mechanics/city/Giza");
            Assert.Equal(HttpStatusCode.OK, cityResponse.StatusCode);
            var gizaList = await cityResponse.Content.ReadFromJsonAsync<List<MechanicProfileDto>>();
            Assert.NotNull(gizaList);
            Assert.Single(gizaList);
            Assert.Equal(gizaId, gizaList[0].Id);
            Assert.Single(gizaList[0].Services);
        }

        [Fact]
        public async Task VehiclesEndpoint_ShouldServeUnifiedMyGarageContract_AndReturnConsistentValidationErrors()
        {
            var client = CreateAuthenticatedClient(Guid.NewGuid(), "User");

            var listResponse = await client.GetAsync("/api/v1/vehicles");
            Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

            // Invalid JSON triggers ApiBehaviorOptions.InvalidModelStateResponseFactory
            var badResponse = await client.PostAsync(
                "/api/v1/vehicles",
                new StringContent("{\"year\":\"not-a-number\"}", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);
            using var doc = JsonDocument.Parse(await badResponse.Content.ReadAsStringAsync());
            Assert.Equal(400, doc.RootElement.GetProperty("statusCode").GetInt32());
            Assert.Equal("Validation failed", doc.RootElement.GetProperty("message").GetString());
            Assert.True(doc.RootElement.GetProperty("errors").GetArrayLength() > 0);
        }

        [Fact]
        public async Task SwaggerOpenApiJson_ShouldGenerateCleanlyWithoutRouteConflicts()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/swagger/v1/swagger.json");
            var body = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, $"Swagger failed: {body}");
            using var doc = JsonDocument.Parse(body);
            var paths = doc.RootElement.GetProperty("paths");

            Assert.True(paths.TryGetProperty("/api/v1/mechanics", out _));
            Assert.True(paths.TryGetProperty("/api/v1/mechanics/city/{city}", out _));
            Assert.True(paths.TryGetProperty("/api/v1/mechanics/search-nearby", out _));
            Assert.True(paths.TryGetProperty("/api/v1/products", out _));
            Assert.True(paths.TryGetProperty("/api/v1/vehicles", out _));
            Assert.True(paths.TryGetProperty("/api/v1/orders", out _));
        }
    }
}
