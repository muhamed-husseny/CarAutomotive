using System.Text.RegularExpressions;
using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Interfaces;

namespace CarAutomotive.Infrastructure.Services
{
    public class ProductFitmentService : IProductFitmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductFitmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FitmentResultDto> CheckFitmentAsync(FitmentCheckRequestDto request)
        {
            if (request == null || request.Vehicle == null)
            {
                return new FitmentResultDto
                {
                    IsCompatible = false,
                    ConfidenceScore = 0,
                    MatchedBy = "NONE",
                    Details = new FitmentDetailsDto
                    {
                        VinMatched = false,
                        MakeTagMatched = false,
                        MetricToleranceCheck = "NOT_EVALUATED",
                        Reason = "Invalid request payload or missing vehicle details."
                    }
                };
            }

            var product = await _unitOfWork.Repository<Product>().GetById(request.ProductId);
            if (product == null)
            {
                return new FitmentResultDto
                {
                    IsCompatible = false,
                    ConfidenceScore = 0,
                    MatchedBy = "NONE",
                    Details = new FitmentDetailsDto
                    {
                        VinMatched = false,
                        MakeTagMatched = false,
                        MetricToleranceCheck = "NOT_EVALUATED",
                        Reason = $"Product with ID '{request.ProductId}' not found."
                    }
                };
            }

            var vehicle = request.Vehicle;
            bool vinMatched = true;

            // Stage 1: Regular Expression VIN Pattern Check
            if (!string.IsNullOrEmpty(product.VinCompatibilityPattern) && product.VinCompatibilityPattern != ".*")
            {
                if (string.IsNullOrEmpty(vehicle.Vin) || !Regex.IsMatch(vehicle.Vin, product.VinCompatibilityPattern, RegexOptions.IgnoreCase))
                {
                    return new FitmentResultDto
                    {
                        IsCompatible = false,
                        ConfidenceScore = 0,
                        MatchedBy = "VIN_REGEX_PATTERN",
                        Details = new FitmentDetailsDto
                        {
                            VinMatched = false,
                            MakeTagMatched = false,
                            MetricToleranceCheck = "NOT_EVALUATED",
                            Reason = $"VIN {vehicle.Vin ?? "N/A"} does not match manufacturer binding pattern: {product.VinCompatibilityPattern}"
                        }
                    };
                }
                vinMatched = true;
            }

            // Stage 2: Millimeter Spec Tolerance Verification
            // Allow +/- 1.0mm variance on Outer Diameter
            string metricToleranceCheck = "PASSED";
            if (product.OuterDiameterMM.HasValue && product.OuterDiameterMM.Value > 0)
            {
                if (vehicle.OuterDiameterMM.HasValue)
                {
                    var diff = Math.Abs(product.OuterDiameterMM.Value - vehicle.OuterDiameterMM.Value);
                    if (diff > 1.0m)
                    {
                        return new FitmentResultDto
                        {
                            IsCompatible = false,
                            ConfidenceScore = 40,
                            MatchedBy = "METRIC_TOLERANCE",
                            Details = new FitmentDetailsDto
                            {
                                VinMatched = vinMatched,
                                MakeTagMatched = true,
                                MetricToleranceCheck = "FAILED",
                                Reason = $"Outer diameter mismatch: Part is {product.OuterDiameterMM.Value}mm, vehicle requires {vehicle.OuterDiameterMM.Value}mm (variance: {diff}mm exceeds +/- 1.0mm)."
                            }
                        };
                    }
                }
            }

            // Stage 3: Success
            return new FitmentResultDto
            {
                IsCompatible = true,
                ConfidenceScore = 100,
                MatchedBy = "VIN_AND_METRIC_MATRIX",
                Details = new FitmentDetailsDto
                {
                    VinMatched = vinMatched,
                    MakeTagMatched = true,
                    MetricToleranceCheck = metricToleranceCheck,
                    Reason = "Vehicle fitment verified successfully."
                }
            };
        }
    }
}
