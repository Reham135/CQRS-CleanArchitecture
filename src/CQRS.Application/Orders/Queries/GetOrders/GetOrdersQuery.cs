using CQRS.Domain.Entities;
using MediatR;

namespace CQRS.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>
{
    public OrderStatus? Status { get; init; }
}

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
