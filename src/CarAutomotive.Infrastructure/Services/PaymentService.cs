using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Core.Entities.Orders;
using CarAutomotive.Core.Entities.Payments;
using CarAutomotive.Core.Enums;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CarAutomotive.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<PaymentIntentResponseDto> CreateOrUpdatePaymentIntentAsync(Guid orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(new OrderByIdWithItemsSpecification(orderId));
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID '{orderId}' was not found.");
            }

            decimal amount = order.TotalAmountEgp > 0 ? order.TotalAmountEgp : order.TotalAmount;
            if (amount <= 0)
            {
                throw new InvalidOperationException("Order total amount must be greater than zero.");
            }

            var secretKey = _config["StripeSettings:SecretKey"] ?? _config["Stripe:SecretKey"];

            var paymentSpec = new PaymentByOrderIdSpecification(orderId);
            var existingPayment = await _unitOfWork.Repository<Payment>().GetEntityWithSpec(paymentSpec);

            string intentId;
            string clientSecret;

            if (!string.IsNullOrWhiteSpace(secretKey))
            {
                StripeConfiguration.ApiKey = secretKey;
                var service = new PaymentIntentService();

                if (existingPayment != null && !string.IsNullOrEmpty(existingPayment.PaymentIntentId) && !existingPayment.PaymentIntentId.StartsWith("pi_sim_"))
                {
                    var updateOptions = new PaymentIntentUpdateOptions
                    {
                        Amount = (long)(amount * 100)
                    };
                    var intent = await service.UpdateAsync(existingPayment.PaymentIntentId, updateOptions);
                    intentId = intent.Id;
                    clientSecret = intent.ClientSecret;
                }
                else
                {
                    var createOptions = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(amount * 100),
                        Currency = "egp",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    var intent = await service.CreateAsync(createOptions);
                    intentId = intent.Id;
                    clientSecret = intent.ClientSecret;
                }
            }
            else
            {
                // Fallback simulation when Stripe key is not configured
                intentId = existingPayment != null && !string.IsNullOrEmpty(existingPayment.PaymentIntentId) 
                    ? existingPayment.PaymentIntentId 
                    : $"pi_sim_{Guid.NewGuid():N}";
                clientSecret = $"{intentId}_secret_{Guid.NewGuid():N}";
            }

            if (existingPayment == null)
            {
                var payment = new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    PaymentIntentId = intentId,
                    ClientSecret = clientSecret,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.Repository<Payment>().Add(payment);
            }
            else
            {
                existingPayment.Amount = amount;
                existingPayment.PaymentIntentId = intentId;
                existingPayment.ClientSecret = clientSecret;
                _unitOfWork.Repository<Payment>().Update(existingPayment);
            }

            await _unitOfWork.CompleteAsync();

            return new PaymentIntentResponseDto
            {
                ClientSecret = clientSecret,
                PaymentIntentId = intentId
            };
        }

        public async Task<bool> HandlePaymentSuccessWebhookAsync(string paymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
                return false;

            var paymentSpec = new PaymentByIntentIdSpecification(paymentIntentId);
            var payment = await _unitOfWork.Repository<Payment>().GetEntityWithSpec(paymentSpec);

            if (payment == null)
                return false;

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(new OrderByIdWithItemsSpecification(payment.OrderId));
            if (order == null)
                return false;

            // 1. Update Payment status
            payment.Status = PaymentStatus.Succeeded;
            _unitOfWork.Repository<Payment>().Update(payment);

            // 2. Update Order status
            order.Status = OrderStatus.Processing;
            order.FulfillmentStatus = "PROCESSING";
            _unitOfWork.Repository<Order>().Update(order);

            // 3. Split Ledger Math (Blueprint Section 6.2)
            decimal subtotal = order.SubtotalEgp > 0 ? order.SubtotalEgp : order.TotalAmount;
            decimal deliveryFee = order.DeliveryFeeEgp;

            decimal platformFee = Math.Round(subtotal * 0.15m, 2);
            decimal vendorPayout = Math.Round(subtotal * 0.85m + deliveryFee, 2);

            // 4. Create Platform Fee Ledger Entry (Admin/System)
            var users = await _unitOfWork.Repository<AppUser>().GetAllAsync();
            var adminUser = users.FirstOrDefault(u => u.Email == "admin2@carautomotive.com" || u.UserName == "admin2") 
                            ?? users.FirstOrDefault();
            var adminUserId = adminUser?.Id ?? order.UserId;

            var platformLedger = new WalletLedger
            {
                UserId = adminUserId,
                ReferenceType = "ORDER",
                ReferenceId = order.OrderId,
                Type = TransactionType.PlatformFee,
                AmountEgp = platformFee,
                Status = TransactionStatus.Completed,
                Description = $"Platform commission fee (15%) for Order {order.OrderNumber}",
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<WalletLedger>().Add(platformLedger);

            // 5. Create Vendor Payout Ledger Entry (Merchant)
            var merchantId = order.Items.FirstOrDefault(i => i.MerchantId.HasValue)?.MerchantId ?? order.UserId;

            var merchantLedger = new WalletLedger
            {
                UserId = merchantId,
                ReferenceType = "ORDER",
                ReferenceId = order.OrderId,
                Type = TransactionType.MerchantPayout,
                AmountEgp = vendorPayout,
                Status = TransactionStatus.Completed,
                Description = $"Vendor payout (85% + delivery) for Order {order.OrderNumber}",
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<WalletLedger>().Add(merchantLedger);

            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Backward compatibility implementations
        public async Task<Payment> CreateOrUpdatePaymentIntent(Guid orderId)
        {
            await CreateOrUpdatePaymentIntentAsync(orderId);
            return await _unitOfWork.Repository<Payment>().GetEntityWithSpec(new PaymentByOrderIdSpecification(orderId))!;
        }

        public async Task<Payment> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            await HandlePaymentSuccessWebhookAsync(paymentIntentId);
            return await _unitOfWork.Repository<Payment>().GetEntityWithSpec(new PaymentByIntentIdSpecification(paymentIntentId))!;
        }

        public async Task<Payment> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var paymentSpec = new PaymentByIntentIdSpecification(paymentIntentId);
            var payment = await _unitOfWork.Repository<Payment>().GetEntityWithSpec(paymentSpec);

            if (payment == null) return null!;

            payment.Status = PaymentStatus.Failed;
            _unitOfWork.Repository<Payment>().Update(payment);
            await _unitOfWork.CompleteAsync();

            return payment;
        }
    }
}
