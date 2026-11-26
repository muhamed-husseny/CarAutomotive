using System.Text.RegularExpressions;

namespace CarAutomotive.Application.Services
{
    public class FitmentResult
    {
        public bool IsCompatible { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class FitmentService
    {
        private static readonly Regex VinRegex = new Regex(
            @"^[A-HJ-NPR-Z0-9]{17}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _unitOfWork;

        public FitmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FitmentResult> CheckFitmentAsync(Guid productId, Guid vehicleId)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var vehicleRepo = _unitOfWork.Repository<Vehicle>();

            var product = await productRepo.GetByIdAsync(productId);
            var vehicle = await vehicleRepo.GetByIdAsync(vehicleId);

            if (product == null || vehicle == null)
            {
                return new FitmentResult { IsCompatible = false, Reason = "Product or vehicle not found" };
            }

            if (!string.IsNullOrEmpty(vehicle.Vin) && !VinRegex.IsMatch(vehicle.Vin))
            {
                return new FitmentResult { IsCompatible = false, Reason = "Invalid VIN format" };
            }

            return new FitmentResult { IsCompatible = true, Reason = "Compatible" };
        }
    }
}