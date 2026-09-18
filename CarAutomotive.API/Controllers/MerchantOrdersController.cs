using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Interfaces;

namespace CarAutomotive.API.Controllers
{
    [Authorize]
    [Route("api/v1/merchant/orders")]
    public class MerchantOrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public MerchantOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /api/v1/merchant/orders
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MerchantOrderDto>>> GetMerchantOrders()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var merchantId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetMerchantOrdersAsync(merchantId);
            return Ok(orders);
        }

        // PUT: /api/v1/merchant/orders/{id:guid}/fulfillment-status
        [HttpPut("{id:guid}/fulfillment-status")]
        public async Task<ActionResult> UpdateFulfillmentStatus(
            [FromRoute] Guid id,
            [FromBody] UpdateFulfillmentStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FulfillmentStatus))
            {
                return BadRequest(new { message = "FulfillmentStatus is required." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var merchantId))
            {
                return Unauthorized();
            }

            var updated = await _orderService.UpdateFulfillmentStatusAsync(merchantId, id, dto.FulfillmentStatus);
            if (!updated)
            {
                return NotFound(new { message = "Order not found or does not contain items for this merchant." });
            }

            return Ok(new { message = "Fulfillment status updated successfully.", status = dto.FulfillmentStatus });
        }
    }
}
