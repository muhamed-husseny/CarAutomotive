namespace CarAutomotive.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string PlateCode { get; set; } = null!;
        public string PlateNumber { get; set; } = null!;
        public int Mileage { get; set; }
        public string? ImageUrl { get; set; }
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}