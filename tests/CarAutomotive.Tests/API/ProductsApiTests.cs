using CarAutomotive.Application.Services;
using CarAutomotive.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CarAutomotive.Tests.API
{
    public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (dbDescriptor != null) services.Remove(dbDescriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("ApiTestDatabase");
                    });

                    var cacheDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IResponseCacheService));
                    if (cacheDescriptor != null) services.Remove(cacheDescriptor);

                    var mockCacheService = new Mock<IResponseCacheService>();
                    mockCacheService
                        .Setup(c => c.GetCachedResponseAsync(It.IsAny<string>()))
                        .ReturnsAsync((string)null);

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
    }
}