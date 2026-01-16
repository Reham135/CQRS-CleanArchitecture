using MediatR;

namespace CQRS.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDetailDto?>;
