using CarAutomotive.Core.Dtos;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GarageController : ControllerBase 
    {
        private readonly IGenericRepository<Vehicle> _vehicleRepo;
        private readonly IMapper _mapper;

        public GarageController(IGenericRepository<Vehicle> vehicleRepo, IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        [HttpGet("my-garage")]
        public async Task<ActionResult<IReadOnlyList<VehicleDto>>> GetMyVehicles()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var spec = new UserVehiclesSpecification(userId);

      
            var vehicles = await _vehicleRepo.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<VehicleDto>>(vehicles);

            return Ok(data);
        }
    }
}