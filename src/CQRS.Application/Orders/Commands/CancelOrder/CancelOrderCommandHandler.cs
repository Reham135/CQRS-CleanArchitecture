using CQRS.Application.Common.Interfaces;
using CQRS.Domain.Entities;
using MediatR;

namespace CQRS.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelOrderResult> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException($"Order with ID {request.OrderId} not found");

        // Domain logic validates cancellation rules
        // Will throw if order is already shipped or delivered
        order.Cancel(request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelOrderResult
        {
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            Message = $"Order cancelled. Reason: {request.Reason}"
        };
    }
}
