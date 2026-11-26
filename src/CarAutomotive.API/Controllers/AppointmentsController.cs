namespace CarAutomotive.API.Controllers
{
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
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

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

        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<AppointmentDto>> UpdateStatus(Guid id, [FromBody] AppointmentStatus newStatus)
        {
            if (!TryGetCurrentUserId(out var requestingUserId))
                return Unauthorized();

            var appointment = await _appointmentService.UpdateAppointmentStatusAsync(
                id,
                newStatus,
                requestingUserId,
                User.IsInRole("Admin"));
            return Ok(appointment);
        }

        [HttpGet("my-appointments")]
        public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMyAppointments()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("mechanic/{mechanicId:guid}")]
        public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMechanicAppointments(Guid mechanicId)
        {
            if (!TryGetCurrentUserId(out var requestingUserId))
                return Unauthorized();

            var appointments = await _appointmentService.GetMechanicAppointmentsAsync(
                mechanicId,
                requestingUserId,
                User.IsInRole("Admin"));
            return Ok(appointments);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
        {
            if (!TryGetCurrentUserId(out var requestingUserId))
                return Unauthorized();

            var appointment = await _appointmentService.GetAppointmentByIdAsync(
                id,
                requestingUserId,
                User.IsInRole("Admin"));
            return Ok(appointment);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out userId);
        }

    }
}
