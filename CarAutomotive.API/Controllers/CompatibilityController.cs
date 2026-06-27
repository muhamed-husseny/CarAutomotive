namespace CarAutomotive.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompatibilityController : BaseApiController
    {

        private readonly ICompatibilityService _service;


        public CompatibilityController(
            ICompatibilityService service)
        {
            _service = service;
        }



        [HttpGet("product/{productId}")]
        public async Task<ActionResult<
            IReadOnlyList<CompatibilityDto>>>
            GetByProduct(int productId)
        {

            var result =
                await _service.GetByProductIdAsync(productId);


            return Ok(result);

        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CompatibilityDto>>
            Create(CreateCompatibilityDto dto)
        {

            var result =
                await _service.CreateAsync(dto);


            return Ok(result);

        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult>
            Delete(int id)
        {

            var deleted =
                await _service.DeleteAsync(id);


            if (!deleted)
                return NotFound();



            return NoContent();

        }

    }
}
