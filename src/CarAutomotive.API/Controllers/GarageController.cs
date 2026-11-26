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
        private readonly IUnitOfWork _unitOfWork;


        public GarageController(
            IGenericRepository<Vehicle> vehicleRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _unitOfWork = unitOfWork;
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
        [HttpPost]
        public async Task<ActionResult<VehicleDto>> AddVehicle(CreateVehicleDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var vehicle = new Vehicle
            {
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                PlateCode = dto.PlateCode,
                PlateNumber = dto.PlateNumber,
                Mileage = dto.Mileage,
                ImageUrl = dto.ImageUrl,
                AppUserId = userId
            };

            _vehicleRepo.Add(vehicle);

            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<VehicleDto>(vehicle));
        }
    }
}