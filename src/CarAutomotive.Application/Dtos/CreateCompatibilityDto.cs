namespace CarAutomotive.Application.Dtos
{
    public class CreateCompatibilityDto
    {
        public int ProductId { get; set; }

        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int Year { get; set; }
    }
}
