namespace CarAutomotive.Core.DTOs
{
    public class PaymentIntentDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
