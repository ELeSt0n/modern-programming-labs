namespace OnlineStore.Domain.Extensions;

public static class OrderExtensions
{
    public static bool IsLargeOrder(this Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return order.Total >= 5000m;
    }

    public static string GetSummary(this Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return $"{order.Customer.Name}: {order.Items.Count} item(s), " +
               $"{order.Total:F2} UAH, {order.Status}";
    }

    // Додаткове завдання: extension block із C# 14.
    extension(Order order)
    {
        public int TotalUnits => order.Items.Sum(item => item.Quantity);

        public bool HasSeveralProducts() => order.Items.Count > 1;
    }
}
