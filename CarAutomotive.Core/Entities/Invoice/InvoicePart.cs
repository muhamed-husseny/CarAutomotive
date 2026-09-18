namespace CarAutomotive.Core.Entities
{
    public class InvoicePart : BaseEntity<Guid>
    {
        public InvoicePart()
        {
            Id = Guid.NewGuid();
        }

        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
