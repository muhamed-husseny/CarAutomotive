namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class NearbyMechanicDto : MechanicProfileDto
    {
        public double DistanceInMeters { get; set; }
        public double DistanceKm { get; set; }
    }
}
