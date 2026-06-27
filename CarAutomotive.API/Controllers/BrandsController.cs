namespace CarAutomotive.API.Controllers
{
    public class BrandsController : BaseApiController
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetBrands()
        {
            var brands = await _brandService.GetBrandsAsync();

            return Ok(brands);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BrandDto>> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);

            if (brand is null)
                return NotFound();

            return Ok(brand);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BrandDto>> CreateBrand(
            CreateBrandDto dto)
        {
            var brand = await _brandService
                .CreateBrandAsync(dto);

            return Ok(brand);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BrandDto>> UpdateBrand(
            int id,
            UpdateBrandDto dto)
        {
            var brand = await _brandService
                .UpdateBrandAsync(id, dto);

            if (brand is null)
                return NotFound();

            return Ok(brand);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var deleted = await _brandService
                .DeleteBrandAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}