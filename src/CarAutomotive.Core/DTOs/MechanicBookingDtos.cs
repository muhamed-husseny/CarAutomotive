namespace CarAutomotive.Core.DTOs
{
    public class MechanicBookingDto
    {
        public Guid Id { get; set; }
        public Guid ClientUserId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Vehicle { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public InvoiceSummaryDto? Invoice { get; set; }
    }

    public class InvoiceSummaryDto
    {
        public Guid Id { get; set; }
        public decimal LaborTotalEgp { get; set; }
        public decimal PartsTotalEgp { get; set; }
        public decimal TotalAmountEgp { get; set; }
        public bool IsPaid { get; set; }
        public List<InvoicePartDto> Parts { get; set; } = new();
    }

    public class UpdateBookingStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class InvoicePartDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CreateInvoiceDto
    {
        public decimal LaborTotal { get; set; }
        public List<InvoicePartDto> Parts { get; set; } = new();
        public bool IsPaid { get; set; }
    }
}
