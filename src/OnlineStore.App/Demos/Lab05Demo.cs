using OnlineStore.App.Subscribers;
using OnlineStore.Domain;
using OnlineStore.Domain.Extensions;
using OnlineStore.Domain.Payments;
using OnlineStore.Domain.Services;

namespace OnlineStore.App.Demos;

internal static class Lab05Demo
{
    public static void Run()
    {
        Action<string> logger = message =>
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");

        var service = new OrderService(logger);
        var notifications = new NotificationService();
        var logging = new LoggingService();

        service.OrderPaid += notifications.Handle;
        service.OrderPaid += logging.Handle;

        var customer = new Customer
        {
            Name = "Anna Kovalenko",
            Email = "anna@example.com",
            Address = new Address("Poltava", "Soborna", "10", "36000")
        };

        var order = new Order(customer);
        service.AddItem(order, new Product("Monitor", 8200m), 1);
        service.AddItem(order, new Product("Mouse", 650m), 2);

        Console.WriteLine("\nFunc discount policies:");
        (string Name, Func<decimal, decimal> Policy)[] policies =
        [
            ("NoDiscount", DiscountPolicies.NoDiscount),
            ("10PercentDiscount", DiscountPolicies.TenPercentDiscount),
            ("20PercentDiscount", DiscountPolicies.TwentyPercentDiscount)
        ];
        foreach (var (name, policy) in policies)
            Console.WriteLine($"{name,-18} {policy(order.Total):F2}");

        Console.WriteLine($"\nPredicate Total >= 5000: {DiscountPolicies.IsLargeOrder(order)}");
        Console.WriteLine($"Extension IsLargeOrder(): {order.IsLargeOrder()}");
        Console.WriteLine($"Extension GetSummary(): {order.GetSummary()}");
        Console.WriteLine($"C# 14 extension: units = {order.TotalUnits}, several products = {order.HasSeveralProducts()}");

        Console.WriteLine("\nPay order (2 subscribers):");
        service.Pay(order, new CardPayment("4242"));

        Console.WriteLine("\nUnsubscribe logging and pay another order (1 subscriber):");
        service.OrderPaid -= logging.Handle;
        var second = new Order(customer);
        service.AddItem(second, new Product("USB cable", 180m), 1);
        service.Pay(second, new BankTransferPayment("INV-2026-002"));

        var third = new Order(customer);
        service.Cancel(third);
    }
}
