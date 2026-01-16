using MediatR;

namespace CQRS.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand : IRequest<CancelOrderResult>
{
    public int OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record CancelOrderResult
{
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
