using CarAutomotive.Application.Services;
using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Tests.Services
{
    public class LedgerServiceTests
    {
        [Theory]
        [InlineData(1000, 150, 850)]
        [InlineData(5000, 750, 4250)]
        [InlineData(0, 0, 0)]
        public void CalculateSplit_WhenGivenTotalAmount_ShouldCalculateCorrectFees(
            decimal totalAmount,
            decimal expectedPlatformFee,
            decimal expectedMerchantPayout)
        {
            var ledgerService = new LedgerService();
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = totalAmount
            };

            // 2. Act
            var result = ledgerService.CalculateSplit(order);

            // 3. Assert 
            Assert.Equal(expectedPlatformFee, result.PlatformFee);
            Assert.Equal(expectedMerchantPayout, result.MerchantPayout);
            Assert.Equal(totalAmount, result.GrossAmount);
        }
    }
}