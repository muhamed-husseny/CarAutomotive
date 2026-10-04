namespace CarAutomotive.Application.Dtos
{
    public class UpdateBrandDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
