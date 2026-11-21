using OnlineStore.Domain;
using OnlineStore.Domain.Payments;

namespace OnlineStore.App.Demos;

internal static class Lab03Demo
{
    public static void Run()
    {
        var address = new Address("Poltava", "Soborna", "10", "36000");
        var sameAddress = new Address("Poltava", "Soborna", "10", "36000");
        var newAddress = address with { Building = "12" };

        Console.WriteLine("Record value equality:");
        Console.WriteLine($"address == sameAddress: {address == sameAddress}");
        Console.WriteLine($"ReferenceEquals: {ReferenceEquals(address, sameAddress)}");
        Console.WriteLine($"with expression: {newAddress}");

        var customer = new Customer
        {
            Name = "Anna Kovalenko",
            Email = "anna@example.com",
            Address = address
        };

        // Nullable: компілятор змушує врахувати відсутність телефону.
        Console.WriteLine($"\nPhone: {customer.PhoneNumber ?? "not specified"}");

        Console.WriteLine("\nOrder categories:");
        foreach (var price in new[] { 499m, 500m, 4999m, 5000m })
        {
            var order = new Order(customer);
            order.AddItem(new Product("Test product", price), 1);
            Console.WriteLine($"{price,8:F2} -> {order.GetOrderCategory()}");
        }

        Console.WriteLine("\nStatus flow and free delivery:");
        var flow = new Order(customer);
        flow.AddItem(new Product("Monitor", 2500m), 1);
        Print(flow);
        flow.Pay(new CardPayment("4242"));
        Print(flow);
        flow.Ship();
        Print(flow);
        flow.Deliver();
        Print(flow);

        var cheap = new Order(customer);
        cheap.AddItem(new Product("Mouse", 650m), 1);
        cheap.Pay(new CardPayment("4242"));
        Console.WriteLine($"Paid order {cheap.Total:F2}: free delivery = {cheap.HasFreeDelivery()}");
    }

    private static void Print(Order order) =>
        Console.WriteLine(
            $"{order.Status,-10} | {order.GetStatusDescription(),-40} | free delivery: {order.HasFreeDelivery()}");
}
