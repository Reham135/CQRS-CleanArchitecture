using CQRS.Application.Common.Interfaces;
using MediatR;

namespace CQRS.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Order> orders;

        if (request.Status.HasValue)
        {
            orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(request.Status.Value, cancellationToken);
        }
        else
        {
            orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        }

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            Status = o.Status.ToString(),
            SubTotal = o.SubTotal,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            TotalAmount = o.TotalAmount,
            ItemCount = o.OrderItems?.Sum(i => i.Quantity) ?? 0
        }).ToList();
    }
}
