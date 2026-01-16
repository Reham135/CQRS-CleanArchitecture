using CQRS.Domain.Entities;

namespace CQRS.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithCategoryAsync(int id, CancellationToken cancellationToken = default);
}
