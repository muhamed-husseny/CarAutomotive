namespace CarAutomotive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _appointmentService;     
        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [EnableRateLimiting("StrictPolicy")]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(userId, dto);
                return Ok(appointment);
            }
            catch (Exception ex) when (ex.Message.Contains("already booked"))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<AppointmentDto>> UpdateStatus(Guid id, [FromBody] AppointmentStatus newStatus)
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointment = await _appointmentService.UpdateAppointmentStatusAsync(id, newStatus, requestingUserId);
            return Ok(appointment);
        }

        [HttpGet("my-appointments")]
        public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMyAppointments()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("mechanic/{mechanicId}")]
        public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMechanicAppointments(Guid mechanicId)
        {
            var appointments = await _appointmentService.GetMechanicAppointmentsAsync(mechanicId);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            return Ok(appointment);
        }

    }
}
