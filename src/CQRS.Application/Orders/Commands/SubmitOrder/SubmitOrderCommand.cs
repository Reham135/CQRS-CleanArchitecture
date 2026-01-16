using MediatR;

namespace CQRS.Application.Orders.Commands.SubmitOrder;

public record SubmitOrderCommand(int OrderId) : IRequest<SubmitOrderResult>;

public record SubmitOrderResult
{
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Message { get; init; } = string.Empty;
}
