using OnlineStore.App.Data;
using OnlineStore.Domain;

namespace OnlineStore.App.Demos;

internal static class Lab04Demo
{
    public static void Run()
    {
        var orders = SeedData.CreateOrders().GetAll();

        // Продажами вважаємо лише фактично оплачені замовлення.
        var sold = orders
            .Where(o => o.Status is OrderStatus.Paid or OrderStatus.Shipped or OrderStatus.Delivered)
            .ToList();

        Console.WriteLine($"Orders in repository: {orders.Count}, sold: {sold.Count}");

        var over1000 = orders.Where(o => o.Total > 1000m).ToList();
        Console.WriteLine($"\n1. Orders with Total > 1000: {over1000.Count}");

        Console.WriteLine("\n2. Top-5 most expensive orders:");
        foreach (var row in orders
                     .OrderByDescending(o => o.Total)
                     .Take(5)
                     .Select(o => new { o.Customer.Name, o.Total, o.Status }))
            Console.WriteLine($"   {row.Name,-16} {row.Total,10:F2}  {row.Status}");

        Console.WriteLine($"\n3. Total sales: {sold.Sum(o => o.Total):F2}");
        Console.WriteLine($"4. Average order value: {sold.Average(o => o.Total):F2}");

        Console.WriteLine("\n5. Orders per customer:");
        foreach (var group in orders
                     .GroupBy(o => o.Customer.Name)
                     .OrderByDescending(g => g.Count())
                     .ThenBy(g => g.Key))
            Console.WriteLine($"   {group.Key,-16} {group.Count()}");

        var allProducts = sold.SelectMany(o => o.Items).Select(i => i.Product.Name).Distinct().Order();
        Console.WriteLine($"\n6. Products in sold orders: {string.Join(", ", allProducts)}");

        Console.WriteLine("\n7. Top-3 products by sold quantity:");
        foreach (var row in sold
                     .SelectMany(o => o.Items)
                     .GroupBy(i => i.Product.Name)
                     .Select(g => new { Name = g.Key, Quantity = g.Sum(i => i.Quantity) })
                     .OrderByDescending(x => x.Quantity)
                     .ThenBy(x => x.Name)
                     .Take(3))
            Console.WriteLine($"   {row.Name,-10} {row.Quantity}");

        Console.WriteLine("\n8. Customers who spent more than 5000:");
        foreach (var row in sold
                     .GroupBy(o => o.Customer.Name)
                     .Select(g => new { Name = g.Key, Spent = g.Sum(o => o.Total) })
                     .Where(x => x.Spent > 5000m)
                     .OrderByDescending(x => x.Spent))
            Console.WriteLine($"   {row.Name,-16} {row.Spent,10:F2}");

        Console.WriteLine($"\n9. Any order > 10000: {orders.Any(o => o.Total > 10000m)}");

        Dictionary<Guid, decimal> spentByCustomer = sold
            .GroupBy(o => o.Customer.Id)
            .ToDictionary(g => g.Key, g => g.Sum(o => o.Total));

        Console.WriteLine("\n10. Dictionary<CustomerId, decimal>:");
        foreach (var (id, spent) in spentByCustomer)
            Console.WriteLine($"   {id.ToString()[..8]}... -> {spent:F2}");
    }
}
