using CarAutomotive.Core.DTOs.AdminDtos;

namespace CarAutomotive.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users-directory")]
        public async Task<ActionResult<IEnumerable<UserDirectoryDto>>> GetUsersDirectory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var directory = await _adminService.GetAllUsersDirectoryAsync(pageNumber, pageSize);
                return Ok(directory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}