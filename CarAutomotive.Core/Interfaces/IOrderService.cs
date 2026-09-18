using CarAutomotive.Core.DTOs;

namespace CarAutomotive.Core.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(Guid clientId, CreateOrderDto dto);
        Task<IReadOnlyList<MerchantOrderDto>> GetMerchantOrdersAsync(Guid merchantId);
        Task<bool> UpdateFulfillmentStatusAsync(Guid merchantId, Guid orderId, string status);
        Task<IReadOnlyList<OrderResponseDto>> GetOrdersForUserAsync(Guid userId, string? status = null);
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId, Guid userId);
        Task<bool> CancelOrderAsync(Guid orderId, Guid userId);
    }
}
