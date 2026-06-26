namespace CarAutomotive.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
    }
}
