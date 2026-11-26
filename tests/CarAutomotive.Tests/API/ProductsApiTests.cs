using System.Net;
using System.Net.Http.Json;
using CarAutomotive.Application.Dtos;
using CarAutomotive.Application.Services;
using CarAutomotive.Core.Entities;
using CarAutomotive.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace CarAutomotive.Tests.API
{
    public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _databaseName = "ApiTestDatabase_" + Guid.NewGuid();

        public ProductsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=car_automotive_test;Username=test;Password=test",
                        ["ConnectionStrings:Redis"] = "localhost:6379",
                        ["Token:Key"] = "TestSuperSecretJwtSigningKeyForUnitTestsOnly123456!",
                        ["Token:Issuer"] = "CarAutomotive.Test",
                        ["Token:Audience"] = "CarAutomotive.TestClient",
                        ["StripeSettings:SecretKey"] = "sk_test_dummy_key_for_tests"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var dbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (dbDescriptor != null) services.Remove(dbDescriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(_databaseName);
                    });

                    var redisDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IConnectionMultiplexer));
                    if (redisDescriptor != null) services.Remove(redisDescriptor);

                    var mockRedis = new Mock<IConnectionMultiplexer>();
                    services.AddSingleton<IConnectionMultiplexer>(mockRedis.Object);

                    var cacheDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IResponseCacheService));
                    if (cacheDescriptor != null) services.Remove(cacheDescriptor);

                    var mockCacheService = new Mock<IResponseCacheService>();
                    mockCacheService
                        .Setup(c => c.GetCachedResponseAsync(It.IsAny<string>()))
                        .ReturnsAsync((string?)null);

                    services.AddSingleton<IResponseCacheService>(mockCacheService.Object);
                });
            });
        }

        [Fact]
        public async Task GetProducts_ShouldReturnSuccessStatusCode_AndJsonContentType()
        {
            // 1. Arrange
            var client = _factory.CreateClient();

            // 2. Act
            var response = await client.GetAsync("/api/products");
            var errorDetails = await response.Content.ReadAsStringAsync();

            // 3. Assert
            Assert.True(response.IsSuccessStatusCode, $"API Failed! Details: {errorDetails}");
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task GetProductById_WithExistingGuid_ShouldReturnOkWithMatchingGuid()
        {
            // 1. Arrange
            var productId = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var category = new Category { Name = "Brakes" };
                var brand = new Brand { Name = "Brembo" };
                db.Categories.Add(category);
                db.Brands.Add(brand);
                await db.SaveChangesAsync();

                db.Products.Add(new Product
                {
                    Id = productId,
                    Title = "Ceramic Brake Pads",
                    BasePriceEgp = 1250m,
                    StockQuantity = 15,
                    CreatedDate = DateTime.UtcNow,
                    CategoryId = category.Id,
                    BrandId = brand.Id
                });
                await db.SaveChangesAsync();
            }

            var client = _factory.CreateClient();

            // 2. Act
            var response = await client.GetAsync($"/api/products/{productId}");

            // 3. Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(product);
            Assert.Equal(productId, product.Id);
            Assert.Equal("Ceramic Brake Pads", product.Name);
        }

        [Fact]
        public async Task GetProductById_WithUnknownGuid_ShouldReturnNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/products/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_WithLegacyIntegerId_ShouldReturnNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/products/123");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}