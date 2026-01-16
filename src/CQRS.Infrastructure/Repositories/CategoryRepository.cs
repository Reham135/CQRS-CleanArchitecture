using CQRS.Application.Common.Interfaces;
using CQRS.Domain.Entities;
using CQRS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetCategoryWithProductsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
