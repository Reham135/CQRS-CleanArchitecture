using MediatR;

namespace CQRS.Application.Orders.Commands.AddItemToOrder;

public record AddItemToOrderCommand : IRequest<AddItemToOrderResult>
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public record AddItemToOrderResult
{
    public decimal SubTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public int TotalItems { get; init; }
}
