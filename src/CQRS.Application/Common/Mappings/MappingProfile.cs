using AutoMapper;
using CQRS.Application.Categories.Queries.GetCategories;
using CQRS.Application.Categories.Queries.GetCategoryById;
using CQRS.Application.Products.Queries.GetProductById;
using CQRS.Application.Products.Queries.GetProducts;
using CQRS.Domain.Entities;

namespace CQRS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryDetailDto>();

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
        CreateMap<Product, ProductDetailDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
    }
}
