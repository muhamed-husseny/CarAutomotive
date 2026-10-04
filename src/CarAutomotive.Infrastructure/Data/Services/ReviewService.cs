namespace CarAutomotive.Infrastructure.Data.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork,IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddReviewAsync(Guid userId, CreateReviewDto dto)
        {
            
            var mechanic = await _unitOfWork.Repository<MechanicProfile>().GetById(dto.MechanicId);
            if (mechanic == null) throw new Exception("Mechanic not found");

            
            var review = new Review
            {
                UserId = userId,
                MechanicId = dto.MechanicId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            
            double totalStars = (mechanic.AverageRating * mechanic.TotalReviews) + dto.Rating;
            mechanic.TotalReviews++;
            mechanic.AverageRating = totalStars / mechanic.TotalReviews;

           
            _unitOfWork.Repository<Review>().Add(review);
            _unitOfWork.Repository<MechanicProfile>().Update(mechanic);

            await _unitOfWork.CompleteAsync();
        }
        public async Task<IReadOnlyList<ReviewDto>> GetMechanicReviewsAsync(Guid mechanicId)
        {
           
            var spec = new ReviewsByMechanicSpecification(mechanicId);
            var reviews = await _unitOfWork.Repository<Review>().ListAsync(spec);

          
            return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
        }
    }
}
