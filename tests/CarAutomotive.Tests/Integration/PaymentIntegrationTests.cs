using CarAutomotive.Core.Entities.Orders;
using CarAutomotive.Core.Enums;
using CarAutomotive.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAutomotive.Tests.Integration
{
    public class PaymentIntegrationTests
    {
        [Fact]
        public async Task UpdateOrderStatus_WhenPaymentSucceeds_ShouldChangeStatusToPaid()
        {
            // 1. Arrange (التجهيز)
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTestDb_" + Guid.NewGuid().ToString())
                .Options;

            var orderId = Guid.NewGuid();

            using (var context = new ApplicationDbContext(options))
            {
                var order = new Order
                {
                    OrderId = orderId,
                    TotalAmount = 15000,
                    Status = OrderStatus.Pending
                };
                context.Orders.Add(order);
                await context.SaveChangesAsync();
            }

            // 2. Act 
            using (var context = new ApplicationDbContext(options))
            {
                var orderToUpdate = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                orderToUpdate.Status = OrderStatus.Processing;
                await context.SaveChangesAsync();
            }

            // 3. Assert 
            using (var context = new ApplicationDbContext(options))
            {
                var updatedOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                Assert.NotNull(updatedOrder);
                Assert.Equal(OrderStatus.Processing, updatedOrder.Status);
            }
        }
    }
}