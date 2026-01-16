using MediatR;

namespace CQRS.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<List<ProductDto>>
{
    public int? CategoryId { get; init; }
}
