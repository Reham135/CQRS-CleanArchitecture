using MediatR;

namespace CQRS.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDetailDto?>;
