namespace CarAutomotive.Application.Dtos
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
