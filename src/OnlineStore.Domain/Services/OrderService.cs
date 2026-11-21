using OnlineStore.Domain.Payments;

namespace OnlineStore.Domain.Services;

/// <summary>Publisher події OrderPaid.</summary>
public sealed class OrderService(Action<string> logger)
{
    private readonly Action<string> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public event EventHandler<Order>? OrderPaid;

    public void AddItem(Order order, Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(order);
        order.AddItem(product, quantity);
        _logger($"Added {product.Name} x {quantity}");
    }

    public void Pay(Order order, IPaymentMethod paymentMethod)
    {
        ArgumentNullException.ThrowIfNull(order);

        // Подія виникає лише після успішної зміни стану.
        var receipt = order.Pay(paymentMethod);
        _logger($"Payment completed: {receipt}");
        OrderPaid?.Invoke(this, order);
    }

    public void Cancel(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        order.Cancel();
        _logger($"Order {order.Id.ToString()[..8]} cancelled");
    }
}
