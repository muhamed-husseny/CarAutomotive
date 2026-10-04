namespace CarAutomotive.Core.Entities
{
    public class Vehicle : BaseEntity<Guid>
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? PlateCode { get; set; }
        public string? PlateNumber { get; set; }
        public int Mileage { get; set; }
        public string? FuelType { get; set; }
        public string? Color { get; set; }
        public VehicleStatus Status { get; set; } = VehicleStatus.Perfect;
        public string? Vin { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}