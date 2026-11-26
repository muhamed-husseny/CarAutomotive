using AutoMapper;
using CarAutomotive.Application.Mapping;
using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities.Appointment;
using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Core.Entities.Mechanic;
using CarAutomotive.Core.Enums;
using CarAutomotive.Infrastructure.Data;
using CarAutomotive.Infrastructure.Data.UnitOfWork;
using CarAutomotive.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NetTopologySuite.Geometries;

namespace CarAutomotive.Tests.Services
{
    public class MechanicBookingServiceTests
    {
        private static async Task<(ApplicationDbContext Context, MechanicBookingService Service, Guid MechanicUserId, Guid AppointmentGuid)> CreateSeededSutAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("MechanicBookingTests_" + Guid.NewGuid())
                .Options;

            var context = new ApplicationDbContext(options);
            var mechanicUserId = Guid.NewGuid();
            var mechanicProfileId = Guid.NewGuid();
            var customerUserId = Guid.NewGuid();
            var appointmentGuid = Guid.NewGuid();

            context.Users.Add(new AppUser
            {
                Id = customerUserId,
                UserName = "booking_customer",
                Email = "customer@test.local",
                FirstName = "Ali",
                LastName = "Samy",
                DisplayName = "Ali Samy"
            });

            context.MechanicProfiles.Add(new MechanicProfile
            {
                Id = mechanicProfileId,
                UserId = mechanicUserId,
                Name = "Maadi Auto Lab",
                PhoneNumber = "01000000001",
                Address = "Maadi, Cairo",
                Location = new Point(31.26, 29.96) { SRID = 4326 }
            });

            context.Appointments.Add(new Appointment
            {
                AppointmentId = appointmentGuid,
                UserId = customerUserId,
                MechanicId = mechanicProfileId,
                AppointmentDate = DateTime.UtcNow.AddDays(1),
                ServiceType = "Brake Overhaul",
                Notes = "Squeaking noise",
                Status = AppointmentStatus.Pending
            });

            await context.SaveChangesAsync();

            var mapper = new MapperConfiguration(
                cfg => cfg.AddProfile<MappingProfiles>(),
                NullLoggerFactory.Instance).CreateMapper();

            var service = new MechanicBookingService(new UnitOfWork(context), mapper);
            return (context, service, mechanicUserId, appointmentGuid);
        }

        [Fact]
        public async Task UpdateBookingStatusAsync_ShouldTransitionLifecycle_AndRejectUnassignedMechanic()
        {
            var (_, service, mechanicUserId, appointmentGuid) = await CreateSeededSutAsync();

            // Unassigned mechanic cannot change booking status
            var unauthorizedResult = await service.UpdateBookingStatusAsync(Guid.NewGuid(), appointmentGuid, "UNDER_REPAIR");
            Assert.False(unauthorizedResult);

            // Assigned mechanic transitions through statuses cleanly
            Assert.True(await service.UpdateBookingStatusAsync(mechanicUserId, appointmentGuid, "UNDER_REPAIR"));
            Assert.True(await service.UpdateBookingStatusAsync(mechanicUserId, appointmentGuid, "READY_FOR_PICKUP"));
            Assert.True(await service.UpdateBookingStatusAsync(mechanicUserId, appointmentGuid, "COMPLETED"));

            var bookings = await service.GetMechanicBookingsAsync(mechanicUserId);
            Assert.Single(bookings);
            Assert.Equal("COMPLETED", bookings[0].Status);
        }

        [Fact]
        public async Task GenerateInvoiceAsync_ShouldComputeItemizedTotals_AndReplacePartsOnUpdate()
        {
            var (_, service, mechanicUserId, appointmentGuid) = await CreateSeededSutAsync();

            // 1. Initial invoice generation
            var created = await service.GenerateInvoiceAsync(mechanicUserId, appointmentGuid, new CreateInvoiceDto
            {
                LaborTotal = 500m,
                IsPaid = false,
                Parts = new List<InvoicePartDto>
                {
                    new() { Name = "Front Brake Pads", Price = 1200m },
                    new() { Name = "Brake Fluid DOT 4", Price = 250m }
                }
            });

            Assert.True(created);
            var afterCreate = await service.GetMechanicBookingsAsync(mechanicUserId);
            Assert.NotNull(afterCreate[0].Invoice);
            Assert.Equal(500m, afterCreate[0].Invoice!.LaborTotalEgp);
            Assert.Equal(1450m, afterCreate[0].Invoice!.PartsTotalEgp);
            Assert.Equal(1950m, afterCreate[0].Invoice!.TotalAmountEgp);
            Assert.False(afterCreate[0].Invoice!.IsPaid);
            Assert.Equal(2, afterCreate[0].Invoice!.Parts.Count);

            // 2. Updating existing invoice replaces parts cleanly without duplicates
            var updated = await service.GenerateInvoiceAsync(mechanicUserId, appointmentGuid, new CreateInvoiceDto
            {
                LaborTotal = 600m,
                IsPaid = true,
                Parts = new List<InvoicePartDto>
                {
                    new() { Name = "Ceramic Brake Pads", Price = 1650m }
                }
            });

            Assert.True(updated);
            var afterUpdate = await service.GetMechanicBookingsAsync(mechanicUserId);
            Assert.Equal(600m, afterUpdate[0].Invoice!.LaborTotalEgp);
            Assert.Equal(1650m, afterUpdate[0].Invoice!.PartsTotalEgp);
            Assert.Equal(2250m, afterUpdate[0].Invoice!.TotalAmountEgp);
            Assert.True(afterUpdate[0].Invoice!.IsPaid);
            Assert.Single(afterUpdate[0].Invoice!.Parts);
            Assert.Equal("Ceramic Brake Pads", afterUpdate[0].Invoice!.Parts[0].Name);
        }
    }
}
