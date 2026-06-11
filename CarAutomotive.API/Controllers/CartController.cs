namespace CarAutomotive.API.Controllers
{
    public class CartController : BaseApiController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /api/cart/{cartId}
        [HttpGet("{cartId}")]
        public async Task<ActionResult<CartDto>> GetCart(string cartId)
        {
            var cart = await _cartService.GetCartAsync(cartId);

            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        // POST: /api/cart/{cartId}/items
        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<CartDto>> AddToCart(
            string cartId,
            AddToCartDto dto)
        {
            var cart = await _cartService.AddToCartAsync(cartId, dto);

            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        // PUT: /api/cart/{cartId}/items
        [HttpPut("{cartId}/items")]
        public async Task<ActionResult<CartDto>> UpdateQuantity(
            string cartId,
            UpdateCartItemDto dto)
        {
            var cart = await _cartService.UpdateQuantityAsync(cartId, dto);

            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        // DELETE: /api/cart/{cartId}/items/{productId}
        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<ActionResult<CartDto>> RemoveFromCart(
            string cartId,
            int productId)
        {
            var cart = await _cartService.RemoveFromCartAsync(cartId, productId);

            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        // DELETE: /api/cart/{cartId}
        [HttpDelete("{cartId}")]
        public async Task<ActionResult> ClearCart(string cartId)
        {
            var result = await _cartService.ClearCartAsync(cartId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}