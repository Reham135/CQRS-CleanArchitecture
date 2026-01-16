using CQRS.Domain.Entities;

namespace CQRS.Application.Common.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int id, CancellationToken cancellationToken = default);
}
