using OnlineStore.Domain;
using OnlineStore.Domain.Payments;

namespace OnlineStore.App.Demos;

internal static class Lab02Demo
{
    public static void Run()
    {
        var customer = new Customer
        {
            Name = "Anna Kovalenko",
            Email = "anna@example.com",
            Address = new Address("Poltava", "Soborna", "10", "36000")
        };
        var laptop = new Product("Laptop", 28500m);
        var mouse = new Product("Mouse", 650m);
        var cable = new Product("USB cable", 180m);

        var cardOrder = new Order(customer);
        cardOrder.AddItem(laptop, 1);
        cardOrder.AddItem(mouse, 2);
        cardOrder.AddItem(cable, 1);
        cardOrder.RemoveItem(cable.Id);
        PrintOrder("Order after AddItem/RemoveItem", cardOrder);

        var transferOrder = new Order(customer);
        transferOrder.AddItem(mouse, 3);

        // Поліморфізм: один виклик через інтерфейс, різні реалізації.
        (Order Order, IPaymentMethod Method)[] payments =
        [
            (cardOrder, new CardPayment("4242")),
            (transferOrder, new BankTransferPayment("INV-2026-001"))
        ];

        Console.WriteLine("\nPolymorphic payment:");
        foreach (var (order, method) in payments)
            Console.WriteLine($"{method.Name,-14} -> {order.Pay(method)}");

        Console.WriteLine("\nDomain rules:");
        TryRule("Pay paid order", () => cardOrder.Pay(new CardPayment("4242")));
        TryRule("Add item to paid order", () => cardOrder.AddItem(mouse, 1));

        var cancelled = new Order(customer);
        cancelled.AddItem(mouse, 1);
        cancelled.Cancel();
        TryRule("Pay cancelled order", () => cancelled.Pay(new CardPayment("4242")));
        TryRule("Product price = 0", () => new Product("Free item", 0m));
        TryRule("Quantity = 0", () => transferOrder.AddItem(mouse, 0));

        PrintOrder("\nOrder state after rejected operations", cardOrder);
    }

    private static void PrintOrder(string title, Order order)
    {
        Console.WriteLine($"{title}: {order.Status}, total {order.Total:F2} UAH");
        foreach (var item in order.Items)
            Console.WriteLine($"  {item.Product.Name} x {item.Quantity} = {item.Total:F2}");
    }

    private static void TryRule(string name, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"{name}: allowed");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"{name}: rejected ({ex.GetType().Name})");
        }
    }
}
