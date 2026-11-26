using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Infrastructure.Data;
using CarAutomotive.Infrastructure.Data.DataSeeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace CarAutomotive.Tests.Integration
{
    public class DemoDataSeederTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _databaseName = "DemoSeederTestDb_" + Guid.NewGuid();

        public DemoDataSeederTests(WebApplicationFactory<Program> factory)
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
                        ["StripeSettings:SecretKey"] = "sk_test_dummy_key_for_tests",
                        ["SeedDemoData"] = "false"
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
        public async Task SeedAsync_WhenExecutedTwice_ShouldBeFullyIdempotentWithZeroDuplicates()
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var sp = scope.ServiceProvider;
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Run twice back-to-back
            await DemoDataSeeder.SeedAsync(db, userManager, roleManager);
            await DemoDataSeeder.SeedAsync(db, userManager, roleManager);

            // Verify 4 flagship demo accounts and roles
            var admin = await userManager.FindByEmailAsync("admin@carautomotive.demo");
            var customer = await userManager.FindByEmailAsync("customer@carautomotive.demo");
            var mechanic = await userManager.FindByEmailAsync("mechanic@carautomotive.demo");
            var merchant = await userManager.FindByEmailAsync("merchant@carautomotive.demo");

            Assert.NotNull(admin);
            Assert.NotNull(customer);
            Assert.NotNull(mechanic);
            Assert.NotNull(merchant);
            Assert.True(await userManager.IsInRoleAsync(admin, "Admin"));
            Assert.True(await userManager.IsInRoleAsync(customer, "User"));
            Assert.True(await userManager.IsInRoleAsync(mechanic, "Mechanic"));
            Assert.True(await userManager.IsInRoleAsync(merchant, "Merchant"));

            // Verify exact counts (zero duplicates)
            var workshops = await db.MechanicProfiles.Include(m => m.Services).ToListAsync();
            Assert.Equal(6, workshops.Count);
            Assert.All(workshops, w =>
            {
                Assert.NotNull(w.Location);
                Assert.Equal(4326, w.Location.SRID);
                Assert.NotEmpty(w.Services);
            });

            Assert.Equal(1, await db.Set<Merchants>().CountAsync());
            Assert.Equal(2, await db.Vehicles.CountAsync());
            Assert.Equal(4, await db.Brands.CountAsync());
            Assert.Equal(4, await db.Categories.CountAsync());
            Assert.Equal(6, await db.Products.CountAsync());
            Assert.True(await db.Compatibilities.CountAsync() >= 14);
        }
    }
}
