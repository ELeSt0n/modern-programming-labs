using OnlineStore.Domain;
using OnlineStore.Domain.Extensions;

namespace OnlineStore.App.Subscribers;

internal sealed class NotificationService
{
    public void Handle(object? sender, Order order) =>
        Console.WriteLine($"[Notification] Email to {order.Customer.Email}: your order is paid.");
}

internal sealed class LoggingService
{
    public void Handle(object? sender, Order order) =>
        Console.WriteLine($"[Log] OrderPaid: {order.GetSummary()}");
}
