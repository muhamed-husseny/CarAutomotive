using System.ComponentModel.DataAnnotations;

namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class MechanicProfileDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } 

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
    }
}
