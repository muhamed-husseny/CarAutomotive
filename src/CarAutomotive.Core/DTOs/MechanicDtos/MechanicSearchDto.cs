namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class MechanicSearchDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double RadiusInKilometers { get; set; } = 5;
    }
}
