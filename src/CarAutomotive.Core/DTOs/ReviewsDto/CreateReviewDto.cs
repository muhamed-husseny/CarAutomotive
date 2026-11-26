using System.ComponentModel.DataAnnotations;

namespace CarAutomotive.Core.DTOs.ReviewsDto
{
    public class CreateReviewDto
    {
        [Required]
        public Guid MechanicId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [Required, MaxLength(500)]
        public string Comment { get; set; }
    }
}
