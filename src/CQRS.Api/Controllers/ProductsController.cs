using CQRS.Application.Products.Commands.CreateProduct;
using CQRS.Application.Products.Commands.DeleteProduct;
using CQRS.Application.Products.Commands.UpdateProduct;
using CQRS.Application.Products.Queries.GetProductById;
using CQRS.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products, optionally filtered by category
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts([FromQuery] int? categoryId)
    {
        var result = await _mediator.Send(new GetProductsQuery { CategoryId = categoryId });
        return Ok(result);
    }

    /// <summary>
    /// Get a product by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> CreateProduct(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id }, id);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id mismatch");
        }

        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
