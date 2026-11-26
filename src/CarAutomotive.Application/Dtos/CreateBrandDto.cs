namespace CarAutomotive.Application.Dtos
{
    public class CreateBrandDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
