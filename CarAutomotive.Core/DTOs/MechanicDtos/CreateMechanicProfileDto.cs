namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class CreateMechanicProfileDto
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

   
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
