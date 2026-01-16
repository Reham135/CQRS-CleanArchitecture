using CQRS.Application.Categories.Commands.CreateCategory;
using CQRS.Application.Categories.Queries.GetCategories;
using CQRS.Application.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var result = await _mediator.Send(new GetCategoriesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get a category by id with its products
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDetailDto>> GetCategory(int id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> CreateCategory(CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategory), new { id }, id);
    }
}
