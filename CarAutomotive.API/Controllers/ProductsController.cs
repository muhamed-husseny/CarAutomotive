using CarAutomotive.API.Helpers;

namespace CarAutomotive.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductDto> _createProductValidator;
        private readonly IValidator<UpdateProductDto> _updateProductValidator;
        private readonly UserManager<AppUser> _userManager;
        public ProductsController(IProductService productService, IValidator<CreateProductDto> createProductValidator,IValidator<UpdateProductDto> updateProductValidator, UserManager<AppUser> userManager)
        {
            _productService = productService;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _userManager = userManager;
        }

        // GET: /api/products
        [Cached(600)]
        [EnableRateLimiting("GeneralPolicy")]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDto>>>
GetProducts([FromQuery] ProductFilterDto filter)
        {
            Guid? userId = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                userId = Guid.Parse(
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier)!);
            }

            var products =
                await _productService
                    .GetProductsAsync(filter, userId);

            return Ok(products);
        }

        // GET: /api/products/{id}
        [EnableRateLimiting("GeneralPolicy")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            var validationResult = await _createProductValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
            var product = await _productService.CreateProductAsync(dto);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
            var validationResult = await _updateProductValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/images")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> UploadProductImage(int id, IFormFile file)
        {
            var imageUrl = await _productService.UploadProductImageAsync(id, file);

            if (imageUrl == null)
                return NotFound();

            return Ok(imageUrl);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId:int}/images/{imageId:int}")]
        public async Task<ActionResult> DeleteProductImage(int productId, int imageId)
        {
            var result = await _productService.DeleteProductImageAsync(productId, imageId);

            if (!result)
                return NotFound();

            return NoContent();
        }

    }
}