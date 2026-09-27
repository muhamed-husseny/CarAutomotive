using CarAutomotive.Application.Services;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Interfaces;
using Moq;

namespace CarAutomotive.Tests.Services
{
    public class FitmentServiceTests
    {
        [Fact]
        public async Task CheckFitmentAsync_WhenVinIsInvalid_ShouldReturnFalse()
        {
            // 1. Arrange
            var vehicleId = Guid.NewGuid();
            var fakeVehicle = new Vehicle
            {
                Id = vehicleId,
                Make = "BMW",
                Vin = "INVALID-VIN-123!@#"
            };

            var productId = Guid.NewGuid();
            var fakeProduct = new Product { Id = productId, Name = "Brake Pads" };

            var mockVehicleRepo = new Mock<IGenericRepository<Vehicle>>();
            var mockProductRepo = new Mock<IGenericRepository<Product>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockVehicleRepo.Setup(repo => repo.GetByIdAsync(vehicleId)).ReturnsAsync(fakeVehicle);
            mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(fakeProduct);

            mockUnitOfWork.Setup(u => u.Repository<Vehicle>()).Returns(mockVehicleRepo.Object);
            mockUnitOfWork.Setup(u => u.Repository<Product>()).Returns(mockProductRepo.Object);

            var fitmentService = new FitmentService(mockUnitOfWork.Object);

            // 2. Act
            var result = await fitmentService.CheckFitmentAsync(productId, vehicleId);

            // 3. Assert
            Assert.False(result.IsCompatible);
            Assert.Contains("Invalid VIN format", result.Reason);
        }
    }
}