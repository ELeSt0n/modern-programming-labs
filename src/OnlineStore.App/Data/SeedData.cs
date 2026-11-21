using OnlineStore.Domain;
using OnlineStore.Domain.Payments;
using OnlineStore.Domain.Repositories;

namespace OnlineStore.App.Data;

internal static class SeedData
{
    public static InMemoryRepository<Order> CreateOrders()
    {
        Customer[] customers =
        [
            NewCustomer("Anna Kovalenko", "anna@example.com", "Poltava"),
            NewCustomer("Bohdan Shevchuk", "bohdan@example.com", "Kyiv"),
            NewCustomer("Olena Marchenko", "olena@example.com", "Lviv"),
            NewCustomer("Taras Bondar", "taras@example.com", "Dnipro"),
            NewCustomer("Iryna Tkachenko", "iryna@example.com", "Odesa"),
            NewCustomer("Maksym Lysenko", "maksym@example.com", "Kharkiv")
        ];

        Product[] p =
        [
            new("Laptop", 28500m),   // 0
            new("Monitor", 8200m),   // 1
            new("SSD", 3100m),       // 2
            new("Headset", 2100m),   // 3
            new("Webcam", 1750m),    // 4
            new("Keyboard", 1450m),  // 5
            new("Mouse", 650m),      // 6
            new("USB cable", 180m)   // 7
        ];

        (int Customer, OrderStatus Status, (int Product, int Quantity)[] Lines)[] plan =
        [
            (0, OrderStatus.Delivered, [(0, 1), (6, 1)]),
            (0, OrderStatus.Paid, [(7, 2)]),
            (0, OrderStatus.Shipped, [(5, 1), (6, 2)]),
            (1, OrderStatus.Delivered, [(1, 2)]),
            (1, OrderStatus.Paid, [(2, 1), (7, 3)]),
            (1, OrderStatus.Created, [(4, 1)]),
            (1, OrderStatus.Delivered, [(6, 4)]),
            (2, OrderStatus.Paid, [(3, 1)]),
            (2, OrderStatus.Cancelled, [(0, 1)]),
            (2, OrderStatus.Shipped, [(2, 2), (5, 1)]),
            (3, OrderStatus.Delivered, [(7, 1)]),
            (3, OrderStatus.Paid, [(4, 1), (6, 1)]),
            (3, OrderStatus.Created, [(6, 2), (7, 2)]),
            (3, OrderStatus.Delivered, [(1, 1), (5, 1)]),
            (4, OrderStatus.Shipped, [(0, 1), (2, 1)]),
            (4, OrderStatus.Paid, [(6, 3)]),
            (4, OrderStatus.Cancelled, [(3, 2)]),
            (5, OrderStatus.Delivered, [(5, 2), (7, 4)]),
            (5, OrderStatus.Paid, [(3, 2), (6, 1)]),
            (5, OrderStatus.Shipped, [(4, 2)])
        ];

        var repository = new InMemoryRepository<Order>();
        var payment = new CardPayment("4242");

        foreach (var (customer, status, lines) in plan)
        {
            var order = new Order(customers[customer]);
            foreach (var (product, quantity) in lines)
                order.AddItem(p[product], quantity);

            ApplyStatus(order, status, payment);
            repository.Add(order);
        }

        return repository;
    }

    private static Customer NewCustomer(string name, string email, string city) => new()
    {
        Name = name,
        Email = email,
        Address = new Address(city, "Central", "1", "00000")
    };

    private static void ApplyStatus(Order order, OrderStatus status, IPaymentMethod payment)
    {
        if (status == OrderStatus.Cancelled)
        {
            order.Cancel();
            return;
        }

        if (status == OrderStatus.Created)
            return;

        order.Pay(payment);
        if (status is OrderStatus.Shipped or OrderStatus.Delivered)
            order.Ship();
        if (status == OrderStatus.Delivered)
            order.Deliver();
    }
}
