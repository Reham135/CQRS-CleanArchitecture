using CQRS.Application.Common.Interfaces;
using CQRS.Domain.Entities;
using MediatR;

namespace CQRS.Application.Orders.Commands.SubmitOrder;

public class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand, SubmitOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SubmitOrderResult> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException($"Order with ID {request.OrderId} not found");

        // Domain logic validates and changes status
        // This will throw DomainException if:
        // - Order is not in Draft status
        // - Order has no items
        // - Total amount is less than $10 (minimum order)
        order.Submit();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitOrderResult
        {
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            Message = "Order submitted successfully. Awaiting approval."
        };
    }
}
