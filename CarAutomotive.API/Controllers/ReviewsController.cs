namespace CarAutomotive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

       
        [Authorize]
        [HttpPost]
        [EnableRateLimiting("StrictPolicy")]
        public async Task<ActionResult> AddReview(CreateReviewDto dto)
        {
           
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _reviewService.AddReviewAsync(userId, dto);
            return Ok(new { message = "Review added successfully!" });
        }

        [HttpGet("mechanic/{mechanicId}")]
        [OutputCache(PolicyName = "Cache5Mins")]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetMechanicReviews(Guid mechanicId)
        {
            var reviews = await _reviewService.GetMechanicReviewsAsync(mechanicId);
            return Ok(reviews);
        }
    }
}
