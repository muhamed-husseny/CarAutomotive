namespace CarAutomotive.Core.Entities
{
    public class Invoice : BaseEntity<Guid>
    {
        public Invoice()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public int AppointmentId { get; set; }
        public Guid AppointmentGuid { get; set; }
        public Appointment.Appointment Appointment { get; set; } = null!;

        public decimal LaborTotalEgp { get; set; }
        public decimal PartsTotalEgp { get; set; }
        public decimal TotalAmountEgp { get; set; }
        public bool IsPaid { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public ICollection<InvoicePart> Parts { get; set; } = new List<InvoicePart>();
    }
}
