using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CarAutomotive.Application.Services;
using CarAutomotive.Core.DTOs.MechanicDtos;
using CarAutomotive.Core.Entities.Appointment;
using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Core.Entities.Mechanic;
using CarAutomotive.Core.Enums;
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
    public class SecurityAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string TestJwtKey = "TestSuperSecretJwtSigningKeyForUnitTestsOnly123456!";
        private const string TestJwtIssuer = "CarAutomotive.Test";
        private const string TestJwtAudience = "CarAutomotive.TestClient";

        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _databaseName = "SecurityTestDatabase_" + Guid.NewGuid();

        public SecurityAuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=car_automotive_test;Username=test;Password=test");
                builder.UseSetting("ConnectionStrings:Redis", "localhost:6379");
                builder.UseSetting("Token:Key", TestJwtKey);
                builder.UseSetting("Token:Issuer", TestJwtIssuer);
                builder.UseSetting("Token:Audience", TestJwtAudience);
                builder.UseSetting("StripeSettings:SecretKey", "sk_test_dummy_key_for_tests");
                builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost:3000");

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
                        ["Cors:AllowedOrigins:0"] = "http://localhost:3000"
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
            var token = GenerateTestJwt(userId, role);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static string GenerateTestJwt(Guid userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, $"{role.ToLowerInvariant()}@test.local"),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = TestJwtIssuer,
                Audience = TestJwtAudience
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(tokenDescriptor));
        }

        [Fact]
        public async Task ProtectedEndpoints_WithoutToken_ShouldReturn401Unauthorized()
        {
            var client = _factory.CreateClient();

            var createMechanic = await client.PostAsJsonAsync("/api/v1/mechanics", new CreateMechanicProfileDto
            {
                Name = "Anon Workshop",
                PhoneNumber = "01000000000",
                Address = "Cairo",
                Latitude = 30.04,
                Longitude = 31.23
            });
            var mechanicBookings = await client.GetAsync("/api/v1/mechanic/bookings");
            var merchantOrders = await client.GetAsync("/api/v1/merchant/orders");
            var appointment = await client.GetAsync($"/api/v1/appointments/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.Unauthorized, createMechanic.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, mechanicBookings.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, merchantOrders.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, appointment.StatusCode);
        }

        [Fact]
        public async Task RoleProtectedPortals_ShouldEnforceMechanicAndMerchantRoles()
        {
            var customerClient = CreateAuthenticatedClient(Guid.NewGuid(), "User");
            var mechanicClient = CreateAuthenticatedClient(Guid.NewGuid(), "Mechanic");
            var merchantClient = CreateAuthenticatedClient(Guid.NewGuid(), "Merchant");

            // Customer cannot access Mechanic or Merchant portals or create mechanics
            Assert.Equal(HttpStatusCode.Forbidden, (await customerClient.GetAsync("/api/v1/mechanic/bookings")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await customerClient.GetAsync("/api/v1/merchant/orders")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await customerClient.PostAsJsonAsync("/api/v1/mechanics", new CreateMechanicProfileDto
            {
                Name = "Blocked Workshop",
                PhoneNumber = "01000000000",
                Address = "Cairo",
                Latitude = 30.04,
                Longitude = 31.23
            })).StatusCode);

            // Mechanic can access Mechanic portal, NOT Merchant portal
            Assert.Equal(HttpStatusCode.OK, (await mechanicClient.GetAsync("/api/v1/mechanic/bookings")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await mechanicClient.GetAsync("/api/v1/merchant/orders")).StatusCode);

            // Merchant can access Merchant portal, NOT Mechanic portal
            Assert.Equal(HttpStatusCode.OK, (await merchantClient.GetAsync("/api/v1/merchant/orders")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await merchantClient.GetAsync("/api/v1/mechanic/bookings")).StatusCode);
        }

        [Fact]
        public async Task Appointments_ShouldPreventIdorAcrossUsersAndAllowOwnerMechanicAndAdmin()
        {
            var ownerUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var mechanicUserId = Guid.NewGuid();
            var mechanicProfileId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Users.Add(new AppUser
                {
                    Id = ownerUserId,
                    UserName = "owner_user",
                    Email = "owner@test.local",
                    FirstName = "Owner",
                    LastName = "User",
                    DisplayName = "Owner User"
                });
                db.MechanicProfiles.Add(new MechanicProfile
                {
                    Id = mechanicProfileId,
                    UserId = mechanicUserId,
                    Name = "Giza Workshop",
                    PhoneNumber = "01011111111",
                    Address = "Dokki, Giza",
                    Location = new Point(31.20, 30.03) { SRID = 4326 }
                });
                db.Appointments.Add(new Appointment
                {
                    AppointmentId = appointmentId,
                    UserId = ownerUserId,
                    MechanicId = mechanicProfileId,
                    AppointmentDate = DateTime.UtcNow.AddDays(1),
                    Notes = "Brake inspection",
                    Status = AppointmentStatus.Pending
                });
                await db.SaveChangesAsync();
            }

            var otherClient = CreateAuthenticatedClient(otherUserId, "User");
            var ownerClient = CreateAuthenticatedClient(ownerUserId, "User");
            var mechanicClient = CreateAuthenticatedClient(mechanicUserId, "Mechanic");
            var adminClient = CreateAuthenticatedClient(Guid.NewGuid(), "Admin");

            // Another user gets 403 Forbidden on appointment read, status update, and mechanic schedule
            Assert.Equal(HttpStatusCode.Forbidden, (await otherClient.GetAsync($"/api/v1/appointments/{appointmentId}")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await otherClient.PutAsJsonAsync($"/api/v1/appointments/{appointmentId}/status", AppointmentStatus.Cancelled)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await otherClient.GetAsync($"/api/v1/appointments/mechanic/{mechanicProfileId}")).StatusCode);

            // Owner, assigned mechanic, and Admin are allowed
            Assert.Equal(HttpStatusCode.OK, (await ownerClient.GetAsync($"/api/v1/appointments/{appointmentId}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await mechanicClient.GetAsync($"/api/v1/appointments/{appointmentId}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await mechanicClient.GetAsync($"/api/v1/appointments/mechanic/{mechanicProfileId}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await adminClient.GetAsync($"/api/v1/appointments/{appointmentId}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await adminClient.GetAsync($"/api/v1/appointments/mechanic/{mechanicProfileId}")).StatusCode);
        }

        [Fact]
        public async Task StripeWebhook_ShouldRemainAllowAnonymous()
        {
            var client = _factory.CreateClient();
            var content = new StringContent("{\"id\":\"evt_test\",\"type\":\"ping\"}", Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/payments/webhook", content);

            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
