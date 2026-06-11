using Stripe;

namespace CarAutomotive.API.Controllers
{
    [Route("api/[controller]")]
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

     
        [HttpPost("{orderId}")]
        public async Task<IActionResult> CreateOrUpdatePaymentIntent(Guid orderId)
        {
            var payment = await _paymentService.CreateOrUpdatePaymentIntent(orderId);

            if (payment == null) return BadRequest("Problem with your payment");

            return Ok(payment);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _config["StripeSettings:WebhookSecret"]);

                PaymentIntent intent;

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment Succeeded: {id}", intent.Id);
                        await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                        break;

                    case EventTypes.PaymentIntentPaymentFailed:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment Failed: {id}", intent.Id);
                        await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                        break;
                }

                return new EmptyResult();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe Webhook Error");
                return BadRequest();
            }
        }
    }
}