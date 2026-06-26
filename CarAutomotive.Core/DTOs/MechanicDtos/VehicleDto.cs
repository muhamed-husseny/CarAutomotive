namespace CarAutomotive.Core.Dtos
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string PlateCode { get; set; }
        public string PlateNumber { get; set; }
        public int Mileage { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}