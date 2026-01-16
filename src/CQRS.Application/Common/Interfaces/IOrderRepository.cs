using CQRS.Domain.Entities;

namespace CQRS.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
