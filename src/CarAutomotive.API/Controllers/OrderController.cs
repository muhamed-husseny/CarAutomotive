using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Interfaces;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [Route("api/[controller]")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderDto> _validator;

        public OrdersController(IOrderService orderService, IValidator<CreateOrderDto> validator)
        {
            _orderService = orderService;
            _validator = validator;
        }

        // POST: /api/v1/orders
        [HttpPost("/api/v1/orders")]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(clientId, dto);
                return Created($"/api/v1/orders/{order.OrderId}", order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: /api/v1/orders
        [HttpGet("/api/v1/orders")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderResponseDto>>> GetOrders([FromQuery] string? status)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersForUserAsync(clientId, status);
            return Ok(orders);
        }

        // GET: /api/v1/orders/{id:guid}
        [HttpGet("/api/v1/orders/{id:guid}")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(id, clientId);
            if (order is null)
                return NotFound();

            return Ok(order);
        }

        // PUT: /api/v1/orders/{id:guid}/cancel
        [HttpPut("/api/v1/orders/{id:guid}/cancel")]
        [HttpPut("{id:guid}/cancel")]
        public async Task<ActionResult> CancelOrder(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var clientId))
            {
                return Unauthorized();
            }

            var result = await _orderService.CancelOrderAsync(id, clientId);
            if (!result)
                return BadRequest(new { message = "Order cannot be cancelled in its current status." });

            return NoContent();
        }
    }
}