using CQRS.Application.Products.Queries.GetProducts;

namespace CQRS.Application.Categories.Queries.GetCategoryById;

public class CategoryDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ProductDto> Products { get; set; } = new();
}
