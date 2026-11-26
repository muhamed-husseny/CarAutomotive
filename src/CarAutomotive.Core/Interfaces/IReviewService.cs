using CarAutomotive.Core.DTOs.ReviewsDto;

namespace CarAutomotive.Core.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(Guid userId, CreateReviewDto dto);
        Task<IReadOnlyList<ReviewDto>> GetMechanicReviewsAsync(Guid mechanicId);
    }
}
