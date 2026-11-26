namespace CarAutomotive.Core.DTOs
{
    public class FitmentCheckRequestDto
    {
        public Guid ProductId { get; set; }
        public VehicleDetailsDto Vehicle { get; set; } = null!;
    }

    public class VehicleDetailsDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? Vin { get; set; }
        public decimal? OuterDiameterMM { get; set; }
    }

    public class FitmentResultDto
    {
        public bool IsCompatible { get; set; }
        public int ConfidenceScore { get; set; }
        public string MatchedBy { get; set; } = null!;
        public FitmentDetailsDto Details { get; set; } = new();
    }

    public class FitmentDetailsDto
    {
        public bool VinMatched { get; set; }
        public bool MakeTagMatched { get; set; }
        public string MetricToleranceCheck { get; set; } = null!;
        public string? Reason { get; set; }
    }
}
