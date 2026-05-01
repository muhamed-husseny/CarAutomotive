namespace CarAutomotive.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProductsController(IGenericRepository<Product> repo , IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET: /api/products
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var spec = new ProductsWithCategorySpec(
                filter.Sort,
                filter.CategoryId,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Search,
                filter.PageIndex,
                filter.PageSize);
            var products = await _repo.GetAllWithSpecAsync(spec);
            var data = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            var countSpec = new ProductsWithFilterForCountSpec(
                filter.CategoryId,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Search);
            var count = await _repo.CountAsync(countSpec);
            return Ok(new Pagination<ProductDto>(
                filter.PageIndex,
                filter.PageSize,
                count,
                data));
        }

        // GET: /api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var spec = new ProductsWithCategorySpec(id);
            var product = await _repo.GetByIdWithSpecAsync(spec);

            if (product==null)
                return NotFound();

            return Ok(_mapper.Map<Product,ProductDto>(product));
        }
        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            _unitOfWork.Repository<Product>().Add(product);
            await _unitOfWork.CompleteAsync();
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id,UpdateProductDto dto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();
            _mapper.Map(dto, product);
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound();
            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

    }
}