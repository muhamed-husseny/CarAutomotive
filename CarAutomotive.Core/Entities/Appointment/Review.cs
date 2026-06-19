namespace CarAutomotive.Core.Entities.Appointment
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; } 
        public string Comment { get; set; }
        public Guid MechanicId { get; set; }
        public MechanicProfile Mechanic { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
