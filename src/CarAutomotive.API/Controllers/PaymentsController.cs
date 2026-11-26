using Stripe;

namespace CarAutomotive.API.Controllers
{
    [Route("api/v1/payments")] 
    [ApiController]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _config;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger, IConfiguration config)
        {
            _paymentService = paymentService;
            _logger = logger;
            _config = config;
        }

        // POST: /api/v1/payments/{orderId}
        [Authorize]
        [HttpPost("{orderId:guid}")] 
        public async Task<ActionResult<PaymentIntentResponseDto>> CreateOrUpdatePaymentIntent(Guid orderId)
        {
            try
            {
                var response = await _paymentService.CreateOrUpdatePaymentIntentAsync(orderId);
                return Ok(response);
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

        // POST: /api/v1/payments/webhook
        [AllowAnonymous]
        [HttpPost("webhook")] 
        public async Task<IActionResult> StripeWebhook()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            try
            {
                var webhookSecret = _config["StripeSettings:WebhookSecret"];
                var signature = Request.Headers["Stripe-Signature"];

                Event stripeEvent;

                if (!string.IsNullOrWhiteSpace(webhookSecret) && !string.IsNullOrWhiteSpace(signature))
                {
                    stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
                }
                else
                {
                    // Fallback parse when signature validation is skipped/testing
                    stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);
                }

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded || stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    var paymentIntentId = intent?.Id ?? string.Empty;

                    _logger.LogInformation("Stripe PaymentIntent Succeeded: {id}", paymentIntentId);
                    await _paymentService.HandlePaymentSuccessWebhookAsync(paymentIntentId);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe Webhook Signature Verification Error: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}