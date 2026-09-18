using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Interfaces;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    [Route("api/v1/mechanic/bookings")]
    public class MechanicBookingsController : BaseApiController
    {
        private readonly IMechanicBookingService _bookingService;

        public MechanicBookingsController(IMechanicBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /api/v1/mechanic/bookings
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MechanicBookingDto>>> GetMechanicBookings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var mechanicId))
            {
                return Unauthorized();
            }

            var bookings = await _bookingService.GetMechanicBookingsAsync(mechanicId);
            return Ok(bookings);
        }

        // PUT: /api/v1/mechanic/bookings/{id}/status
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult> UpdateBookingStatus(
            [FromRoute] Guid id,
            [FromBody] UpdateBookingStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { message = "Status is required." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var mechanicId))
            {
                return Unauthorized();
            }

            var success = await _bookingService.UpdateBookingStatusAsync(mechanicId, id, dto.Status);
            if (!success)
            {
                return NotFound(new { message = "Booking not found or not assigned to this mechanic." });
            }

            return Ok(new { message = "Booking status updated successfully.", status = dto.Status });
        }

        // POST: /api/v1/mechanic/bookings/{id}/invoice
        [HttpPost("{id:guid}/invoice")]
        public async Task<ActionResult> GenerateInvoice(
            [FromRoute] Guid id,
            [FromBody] CreateInvoiceDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Invoice data is required." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var mechanicId))
            {
                return Unauthorized();
            }

            var success = await _bookingService.GenerateInvoiceAsync(mechanicId, id, dto);
            if (!success)
            {
                return NotFound(new { message = "Booking not found or not assigned to this mechanic." });
            }

            return Ok(new { message = "Invoice generated successfully." });
        }
    }
}
