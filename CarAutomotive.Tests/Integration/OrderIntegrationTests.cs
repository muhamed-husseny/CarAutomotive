using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Orders;
using CarAutomotive.Core.Enums;
using CarAutomotive.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAutomotive.Tests.Integration
{
    public class OrderIntegrationTests
    {
        [Fact]
        public async Task CreateOrder_ShouldSaveSuccessfully_WithCorrectRelationships()
        {
            // 1. Arrange 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AutomateTestDb_" + Guid.NewGuid().ToString())
                .Options;

            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            using (var context = new ApplicationDbContext(options))
            {
                var product = new Product
                {
                    Id = productId,
                    Name = "Turbo Charger",
                    Price = 5000,
                    StockCount = 10
                };
                context.Products.Add(product);

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = userId,
                    TotalAmount = 5000,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = productId,
                            ProductName = "Turbo Charger",
                            Price = 5000,
                            Quantity = 1
                        }
                    }
                };

                // 2. Act 
                context.Orders.Add(order);
                await context.SaveChangesAsync();
            }

            // 3. Assert 
            using (var context = new ApplicationDbContext(options))
            {
                var savedOrder = await context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync();

                Assert.NotNull(savedOrder);
                Assert.Single(savedOrder.Items);
                Assert.Equal(5000, savedOrder.TotalAmount);
                Assert.Equal("Turbo Charger", savedOrder.Items.First().ProductName);
            }
        }
    }
}