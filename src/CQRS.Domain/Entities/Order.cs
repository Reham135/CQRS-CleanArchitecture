namespace CQRS.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Domain Logic - Complex business rules
    public void CalculateTotals()
    {
        SubTotal = OrderItems.Sum(x => x.Quantity * x.UnitPrice);
        DiscountAmount = CalculateDiscount();
        TaxAmount = (SubTotal - DiscountAmount) * 0.10m; // 10% tax
        TotalAmount = SubTotal - DiscountAmount + TaxAmount;
    }

    private decimal CalculateDiscount()
    {
        // Business Rule: 10% discount for orders over $500
        if (SubTotal >= 500)
            return SubTotal * 0.10m;

        // Business Rule: 5% discount for orders with 5+ items
        if (OrderItems.Sum(x => x.Quantity) >= 5)
            return SubTotal * 0.05m;

        return 0;
    }

    public void AddItem(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var existingItem = OrderItems.FirstOrDefault(x => x.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.UpdateLineTotal();
        }
        else
        {
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            };
            orderItem.UpdateLineTotal();
            OrderItems.Add(orderItem);
        }

        CalculateTotals();
    }

    public void RemoveItem(int productId)
    {
        var item = OrderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
            throw new DomainException($"Product {productId} not found in order");

        OrderItems.Remove(item);
        CalculateTotals();
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Only draft orders can be submitted");

        if (!OrderItems.Any())
            throw new DomainException("Cannot submit an empty order");

        // Business Rule: Minimum order amount
        if (TotalAmount < 10)
            throw new DomainException("Minimum order amount is $10");

        Status = OrderStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != OrderStatus.Submitted)
            throw new DomainException("Only submitted orders can be approved");

        Status = OrderStatus.Approved;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel shipped or delivered orders");

        Status = OrderStatus.Cancelled;
        Notes = $"Cancelled: {reason}";
    }

    public static Order Create()
    {
        return new Order
        {
            OrderNumber = GenerateOrderNumber(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Draft
        };
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

public enum OrderStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
