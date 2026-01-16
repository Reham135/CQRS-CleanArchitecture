using CQRS.Application.Common.Interfaces;
using CQRS.Domain.Entities;
using MediatR;

namespace CQRS.Application.Orders.Commands.AddItemToOrder;

public class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand, AddItemToOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddItemToOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AddItemToOrderResult> Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
    {
        // Get order with items
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException($"Order with ID {request.OrderId} not found");

        // Business rule: Can only add items to draft orders
        if (order.Status != OrderStatus.Draft)
            throw new DomainException("Can only add items to draft orders");

        // Get product
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new DomainException($"Product with ID {request.ProductId} not found");

        // Domain logic handles adding item and recalculating totals
        order.AddItem(product, request.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddItemToOrderResult
        {
            SubTotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            TotalItems = order.OrderItems.Sum(x => x.Quantity)
        };
    }
}
