using CQRS.Application.Orders.Commands.AddItemToOrder;
using CQRS.Application.Orders.Commands.CancelOrder;
using CQRS.Application.Orders.Commands.CreateOrder;
using CQRS.Application.Orders.Commands.SubmitOrder;
using CQRS.Application.Orders.Queries.GetOrderById;
using CQRS.Application.Orders.Queries.GetOrders;
using CQRS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all orders, optionally filtered by status
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] OrderStatus? status)
    {
        var result = await _mediator.Send(new GetOrdersQuery { Status = status });
        return Ok(result);
    }

    /// <summary>
    /// Get order by ID with all items
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new order with items
    /// Demonstrates: Domain logic for discounts calculation
    /// - 10% discount for orders over $500
    /// - 5% discount for 5+ items
    /// - 10% tax applied after discount
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);
    }

    /// <summary>
    /// Add item to existing draft order
    /// Demonstrates: Domain validation (only draft orders can be modified)
    /// </summary>
    [HttpPost("{id}/items")]
    public async Task<ActionResult<AddItemToOrderResult>> AddItem(int id, AddItemRequest request)
    {
        var command = new AddItemToOrderCommand
        {
            OrderId = id,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Submit order for processing
    /// Demonstrates: Domain state transition rules
    /// - Only draft orders can be submitted
    /// - Minimum order amount is $10
    /// - Order must have at least one item
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<SubmitOrderResult>> SubmitOrder(int id)
    {
        var result = await _mediator.Send(new SubmitOrderCommand(id));
        return Ok(result);
    }

    /// <summary>
    /// Cancel an order
    /// Demonstrates: Domain business rules
    /// - Cannot cancel shipped or delivered orders
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<CancelOrderResult>> CancelOrder(int id, CancelOrderRequest request)
    {
        var command = new CancelOrderCommand
        {
            OrderId = id,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public record AddItemRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public record CancelOrderRequest
{
    public string Reason { get; init; } = string.Empty;
}
