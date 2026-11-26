namespace CarAutomotive.Core.Dtos
{
    public class CreateVehicleDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? PlateCode { get; set; }
        public string? PlateNumber { get; set; }
        public int Mileage { get; set; }
        public string? FuelType { get; set; }
        public string? Color { get; set; }
        public string? Vin { get; set; }
        public List<string> Images { get; set; } = new();
        public string? ImageUrl { get; set; }
    }
}
