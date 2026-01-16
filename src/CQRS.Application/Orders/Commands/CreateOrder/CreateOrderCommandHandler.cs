using CQRS.Application.Common.Interfaces;
using CQRS.Domain.Entities;
using MediatR;

namespace CQRS.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Start transaction for complex operation
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Create order using domain factory method
            var order = Order.Create();

            // Add items using domain logic
            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                    throw new DomainException($"Product with ID {item.ProductId} not found");

                // Domain logic handles validation and calculations
                order.AddItem(product, item.Quantity);
            }

            // Persist order
            _unitOfWork.Orders.Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Determine discount reason for response
            var discountReason = GetDiscountReason(order);

            return new CreateOrderResult
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                DiscountReason = discountReason
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static string GetDiscountReason(Order order)
    {
        if (order.DiscountAmount == 0)
            return "No discount applied";

        if (order.SubTotal >= 500)
            return "10% discount for orders over $500";

        if (order.OrderItems.Sum(x => x.Quantity) >= 5)
            return "5% discount for ordering 5+ items";

        return "No discount applied";
    }
}
