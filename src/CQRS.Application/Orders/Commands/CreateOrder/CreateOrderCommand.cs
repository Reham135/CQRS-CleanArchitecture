using MediatR;

namespace CQRS.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<CreateOrderResult>
{
    public List<OrderItemRequest> Items { get; init; } = new();
}

public record OrderItemRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public record CreateOrderResult
{
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string DiscountReason { get; init; } = string.Empty;
}
