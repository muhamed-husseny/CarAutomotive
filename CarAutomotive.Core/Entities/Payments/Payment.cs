namespace CarAutomotive.Core.Entities.Payments
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        
        public string PaymentIntentId { get; set; }

        public string ClientSecret { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
