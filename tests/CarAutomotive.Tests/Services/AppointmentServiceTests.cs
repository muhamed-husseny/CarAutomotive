using AutoMapper;
using CarAutomotive.Application.MappingProfiles;
using CarAutomotive.Core.DTOs.AppointmentsDto;
using CarAutomotive.Core.Entities.Appointment;
using CarAutomotive.Core.Enums;
using CarAutomotive.Infrastructure.Data;
using CarAutomotive.Infrastructure.Data.Services;
using CarAutomotive.Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarAutomotive.Tests.Services
{
    public class AppointmentServiceTests
    {
        private static (ApplicationDbContext Context, AppointmentService Service) CreateSut()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("AppointmentTests_" + Guid.NewGuid())
                .Options;

            var context = new ApplicationDbContext(options);
            var unitOfWork = new UnitOfWork(context);
            var mapperConfig = new MapperConfiguration(
                cfg => cfg.AddProfile<AppointmenMappingProfile>(),
                NullLoggerFactory.Instance);
            var mapper = mapperConfig.CreateMapper();

            return (context, new AppointmentService(unitOfWork, mapper));
        }

        [Fact]
        public async Task CreateAppointmentAsync_WhenDateIsInPast_ShouldThrowException()
        {
            var (_, service) = CreateSut();
            var dto = new CreateAppointmentDto
            {
                MechanicId = Guid.NewGuid(),
                AppointmentDate = DateTime.UtcNow.AddHours(-2),
                Notes = "Oil change"
            };

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.CreateAppointmentAsync(Guid.NewGuid(), dto));

            Assert.Contains("in the past", ex.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_WhenSlotAlreadyBookedForMechanic_ShouldRejectOverlap()
        {
            var (_, service) = CreateSut();
            var mechanicId = Guid.NewGuid();
            var slot = DateTime.UtcNow.AddDays(2);

            await service.CreateAppointmentAsync(Guid.NewGuid(), new CreateAppointmentDto
            {
                MechanicId = mechanicId,
                AppointmentDate = slot,
                Notes = "First booking"
            });

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.CreateAppointmentAsync(Guid.NewGuid(), new CreateAppointmentDto
                {
                    MechanicId = mechanicId,
                    AppointmentDate = slot,
                    Notes = "Conflicting booking"
                }));

            Assert.Contains("already booked", ex.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_WhenExistingSlotIsCancelled_ShouldAllowRebooking()
        {
            var (context, service) = CreateSut();
            var mechanicId = Guid.NewGuid();
            var slot = DateTime.UtcNow.AddDays(3);

            context.Appointments.Add(new Appointment
            {
                UserId = Guid.NewGuid(),
                MechanicId = mechanicId,
                AppointmentDate = slot,
                Notes = "Cancelled booking",
                Status = AppointmentStatus.Cancelled
            });
            await context.SaveChangesAsync();

            var created = await service.CreateAppointmentAsync(Guid.NewGuid(), new CreateAppointmentDto
            {
                MechanicId = mechanicId,
                AppointmentDate = slot,
                Notes = "New booking on freed slot"
            });

            Assert.NotNull(created);
            Assert.Equal("New booking on freed slot", created.Notes);
        }
    }
}
