using NetTopologySuite.Geometries;

namespace CarAutomotive.Core.Entities.Mechanic
{
    public class MechanicProfile : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

     
        public Point Location { get; set; }

        public double AverageRating { get; set; } 
        public int TotalReviews { get; set; } 
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; } = true; 

        
        public ICollection<MechanicService> Services { get; set; } = new List<MechanicService>();
    }
}
