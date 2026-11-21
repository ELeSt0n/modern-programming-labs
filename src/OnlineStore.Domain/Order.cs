using OnlineStore.Domain.Payments;

namespace OnlineStore.Domain;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Order(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        Id = Guid.NewGuid();
        Customer = customer;
        Status = OrderStatus.Created;
    }

    public Guid Id { get; }
    public Customer Customer { get; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.Total);

    public void AddItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        EnsureCreated();

        var existing = _items.FirstOrDefault(item => item.Product.Id == product.Id);
        if (existing is null)
            _items.Add(new OrderItem(product, quantity));
        else
            existing.Increase(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        EnsureCreated();

        var item = _items.FirstOrDefault(x => x.Product.Id == productId)
            ?? throw new InvalidOperationException("Product is not in the order.");
        _items.Remove(item);
    }

    public string Pay(IPaymentMethod paymentMethod)
    {
        ArgumentNullException.ThrowIfNull(paymentMethod);

        if (Status is OrderStatus.Paid or OrderStatus.Shipped or OrderStatus.Delivered)
            throw new InvalidOperationException("Order is already paid.");
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cancelled order cannot be paid.");
        if (_items.Count == 0)
            throw new InvalidOperationException("Empty order cannot be paid.");

        var receipt = paymentMethod.Pay(Total);
        Status = OrderStatus.Paid;
        return receipt;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled.");
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Paid order cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Only a paid order can be shipped.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only a shipped order can be delivered.");

        Status = OrderStatus.Delivered;
    }

    public string GetStatusDescription() => Status switch
    {
        OrderStatus.Created => "Order is created and waits for payment",
        OrderStatus.Paid => "Order is paid and being prepared",
        OrderStatus.Shipped => "Order is on the way",
        OrderStatus.Delivered => "Order is delivered",
        OrderStatus.Cancelled => "Order is cancelled",
        _ => "Unknown status"
    };

    public OrderCategory GetOrderCategory() => Total switch
    {
        < 500m => OrderCategory.Small,
        < 5000m => OrderCategory.Medium,
        _ => OrderCategory.Large
    };

    /// <summary>Property pattern: оплачене замовлення від 2000 грн доставляється безкоштовно.</summary>
    public bool HasFreeDelivery() => this is { Status: OrderStatus.Paid, Total: >= 2000m };

    private void EnsureCreated()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Items can be changed only in a new order.");
    }
}
