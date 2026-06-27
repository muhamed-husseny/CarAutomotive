public class MechanicProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}