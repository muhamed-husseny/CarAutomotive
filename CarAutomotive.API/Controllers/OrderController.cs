using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderDto> _validator;

        public OrdersController(IOrderService orderService, IValidator<CreateOrderDto> validator)
        {
            _orderService = orderService;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(
            [FromBody] CreateOrderDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderService
                .CreateOrderAsync(userId, dto);

            if (order is null)
                return BadRequest("Unable to create order");

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrders([FromQuery] OrderStatus? status)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var orders = await _orderService
                .GetOrdersForUserAsync(
                    userId,
                    status);

            return Ok(orders);
        }

        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(
            Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderService
                .GetOrderByIdAsync(id, userId);

            if (order is null)
                return NotFound();

            return Ok(order);
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<ActionResult> CancelOrder(
            Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _orderService
                .CancelOrderAsync(id, userId); 

            if (!result)
                return BadRequest();

            return NoContent();
        }
    }
}