public class MechanicServiceDto
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal EstimatedPrice { get; set; }
}

public class MechanicProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsAvailable { get; set; }
    public List<MechanicServiceDto> Services { get; set; } = new();
}