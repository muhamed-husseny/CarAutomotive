using CarAutomotive.Core.Dtos;
using CarAutomotive.Core.Interfaces;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class VehiclesController : BaseApiController
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VehicleDto>>> GetVehicles()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            var vehicles = await _vehicleService.GetClientVehiclesAsync(clientId);
            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> AddVehicle([FromBody] CreateVehicleDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            var vehicle = await _vehicleService.AddVehicleAsync(clientId, dto);
            return StatusCode(StatusCodes.Status201Created, vehicle);
        }
    }
}
