using CQRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // Only seed if the database is empty
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            },
            new Category
            {
                Name = "Clothing",
                Description = "Apparel and fashion items"
            },
            new Category
            {
                Name = "Books",
                Description = "Physical and digital books"
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            // Electronics
            new Product
            {
                Name = "Smartphone",
                Description = "Latest model smartphone with 5G capability",
                Price = 999.99m,
                CategoryId = categories[0].Id
            },
            new Product
            {
                Name = "Laptop",
                Description = "High-performance laptop for work and gaming",
                Price = 1499.99m,
                CategoryId = categories[0].Id
            },
            // Clothing
            new Product
            {
                Name = "T-Shirt",
                Description = "Comfortable cotton t-shirt",
                Price = 29.99m,
                CategoryId = categories[1].Id
            },
            new Product
            {
                Name = "Jeans",
                Description = "Classic blue denim jeans",
                Price = 79.99m,
                CategoryId = categories[1].Id
            },
            // Books
            new Product
            {
                Name = "Clean Architecture",
                Description = "A Craftsman's Guide to Software Structure and Design by Robert C. Martin",
                Price = 34.99m,
                CategoryId = categories[2].Id
            },
            new Product
            {
                Name = "Domain-Driven Design",
                Description = "Tackling Complexity in the Heart of Software by Eric Evans",
                Price = 54.99m,
                CategoryId = categories[2].Id
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
