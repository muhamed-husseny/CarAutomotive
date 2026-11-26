using Microsoft.AspNetCore.OutputCaching;

namespace CarAutomotive.API.Controllers
{
    [Route("api/v1/mechanics")]
    public class MechanicController : BaseApiController
    {
        private readonly IMechanicService _mechanicService;

        public MechanicController(IMechanicService mechanicService)
        {
            _mechanicService = mechanicService;
        }

        [Authorize(Roles = "Admin,Mechanic")]
        [HttpPost]
        [EnableRateLimiting("StrictPolicy")]
        public async Task<ActionResult<MechanicProfileDto>> CreateMechanic(CreateMechanicProfileDto dto)
        {
            var mechanic = await _mechanicService.CreateMechanicProfileAsync(dto);
            return Ok(mechanic);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MechanicProfileDto>> GetMechanicById(Guid id)
        {
            var mechanic = await _mechanicService.GetMechanicProfileByIdAsync(id);

            if (mechanic == null)
                return NotFound(new ApiResponse(404, "Mechanic not found."));

            return Ok(mechanic);
        }

        [HttpGet("search-nearby")]
        public async Task<ActionResult<IReadOnlyList<NearbyMechanicDto>>> SearchNearbyMechanics([FromQuery] MechanicSearchDto searchDto)
        {
            var mechanics = await _mechanicService.SearchNearbyMechanicsAsync(searchDto);
            return Ok(mechanics);
        }

        [HttpGet]
        [OutputCache(PolicyName = "Cache5Mins")]
        public async Task<ActionResult<IReadOnlyList<MechanicProfileDto>>> GetAllMechanics(
            [FromQuery] string? city = null,
            [FromQuery] double? minRating = null,
            [FromQuery] string? serviceName = null)
        {
            var mechanics = await _mechanicService.GetAllMechanicsAsync(city, minRating, serviceName);
            return Ok(mechanics);
        }

        [HttpGet("city/{city}")]
        [OutputCache(PolicyName = "Cache5Mins")]
        public async Task<ActionResult<IReadOnlyList<MechanicProfileDto>>> GetMechanicsByCity(string city)
        {
            var mechanics = await _mechanicService.GetMechanicsByCityAsync(city);
            return Ok(mechanics);
        }
    }

}
