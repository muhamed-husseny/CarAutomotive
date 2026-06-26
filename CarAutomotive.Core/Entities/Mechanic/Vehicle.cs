namespace CarAutomotive.Core.Entities
{
    public class Vehicle : BaseEntity 
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string PlateCode { get; set; }
        public string PlateNumber { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; }
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}