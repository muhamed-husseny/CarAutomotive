using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Application.Services
{
    public class LedgerEntry
    {
        public Guid OrderId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal MerchantPayout { get; set; }
    }

    public class LedgerService
    {
        private const decimal PlatformCommissionRate = 0.15m;

        public LedgerEntry CalculateSplit(Order order)
        {
            var platformFee = Math.Round(order.TotalAmount * PlatformCommissionRate, 2);
            var merchantPayout = order.TotalAmount - platformFee;

            return new LedgerEntry
            {
                OrderId = order.OrderId,
                GrossAmount = order.TotalAmount,
                PlatformFee = platformFee,
                MerchantPayout = merchantPayout
            };
        }
    }
}