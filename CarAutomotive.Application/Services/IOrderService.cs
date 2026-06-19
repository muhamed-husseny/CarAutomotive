using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Application.Services
{
    public interface IOrderService
    {
        Task<OrderToReturnDto?> CreateOrderAsync(
            Guid userId,
            CreateOrderDto dto);

        Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(
            Guid userId,
            OrderStatus? status);

        Task<OrderToReturnDto?> GetOrderByIdAsync(
            Guid orderId, 
            Guid userId);

        Task<bool> CancelOrderAsync(
            Guid orderId, 
            Guid userId);
    }
}