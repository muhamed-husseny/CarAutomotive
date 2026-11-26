using AutoMapper;
using CarAutomotive.Application.Dtos;
using CarAutomotive.Application.Services;
using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.DTOs.MechanicDtos;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Mechanic;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetTopologySuite.Geometries;
using StackExchange.Redis;

namespace CarAutomotive.Tests.API
{
    public class DependencyInjectionTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _databaseName = "DiTestDatabase_" + Guid.NewGuid();

        public DependencyInjectionTests(WebApplicationFactory<Program> factory)
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
                        options.UseInMemoryDatabase(_databaseName));

                    var redisDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IConnectionMultiplexer));
                    if (redisDescriptor != null) services.Remove(redisDescriptor);

                    services.AddSingleton(new Mock<IConnectionMultiplexer>().Object);
                });
            });
        }

        [Fact]
        public async Task ServiceContainer_ShouldResolveAllCoreAndApplicationServices()
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var sp = scope.ServiceProvider;

            Assert.NotNull(sp.GetRequiredService<IUnitOfWork>());
            Assert.NotNull(sp.GetRequiredService<IProductService>());
            Assert.NotNull(sp.GetRequiredService<ICartService>());
            Assert.NotNull(sp.GetRequiredService<IBrandService>());
            Assert.NotNull(sp.GetRequiredService<ICategoryService>());
            Assert.NotNull(sp.GetRequiredService<ICompatibilityService>());
            Assert.NotNull(sp.GetRequiredService<IAdminService>());
            Assert.NotNull(sp.GetRequiredService<IMechanicService>());
            Assert.NotNull(sp.GetRequiredService<IAppointmentService>());
            Assert.NotNull(sp.GetRequiredService<IReviewService>());
            Assert.NotNull(sp.GetRequiredService<IPaymentService>());
            Assert.NotNull(sp.GetRequiredService<IVehicleService>());
            Assert.NotNull(sp.GetRequiredService<IProductFitmentService>());
            Assert.NotNull(sp.GetRequiredService<IOrderService>());
            Assert.NotNull(sp.GetRequiredService<IMechanicBookingService>());
            Assert.NotNull(sp.GetRequiredService<ITokenService>());
        }

        [Fact]
        public void AutoMapper_ShouldDiscoverAllProfilesInApplicationAssembly()
        {
            using var scope = _factory.Services.CreateScope();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var productId = Guid.NewGuid();
            var productDto = mapper.Map<ProductDto>(new Product
            {
                Id = productId,
                Title = "Oil Filter",
                BasePriceEgp = 150m,
                Category = new Category { Name = "Engine" },
                Brand = new Brand { Name = "Bosch" }
            });

            Assert.Equal(productId, productDto.Id);
            Assert.Equal("Oil Filter", productDto.Name);
            Assert.Equal("Engine", productDto.CategoryName);
            Assert.Equal("Bosch", productDto.BrandName);

            var mechanicDto = mapper.Map<MechanicProfileDto>(new MechanicProfile
            {
                Id = Guid.NewGuid(),
                Name = "Maadi Auto Care",
                Address = "Maadi, Cairo",
                PhoneNumber = "01000000000",
                Location = new Point(31.26, 29.96) { SRID = 4326 }
            });

            Assert.Equal("Maadi Auto Care", mechanicDto.Name);
            Assert.Equal(31.26, mechanicDto.Longitude, precision: 2);
            Assert.Equal(29.96, mechanicDto.Latitude, precision: 2);
        }

        [Fact]
        public void FluentValidation_ShouldResolveValidatorsFromBothApplicationAndCoreAssemblies()
        {
            using var scope = _factory.Services.CreateScope();
            var sp = scope.ServiceProvider;

            // From CarAutomotive.Application
            Assert.NotNull(sp.GetRequiredService<IValidator<CreateProductDto>>());
            Assert.NotNull(sp.GetRequiredService<IValidator<UpdateProductDto>>());
            Assert.NotNull(sp.GetRequiredService<IValidator<CreateOrderDto>>());

            // From CarAutomotive.Core
            Assert.NotNull(sp.GetRequiredService<IValidator<CreateMechanicProfileDto>>());
            Assert.NotNull(sp.GetRequiredService<IValidator<MechanicSearchDto>>());
        }
    }
}
