namespace CarAutomotive.Core.Entities.Payments
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        
        public string PaymentIntentId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
