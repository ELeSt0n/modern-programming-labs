namespace OnlineStore.Domain.Services;

public static class DiscountPolicies
{
    public static readonly Func<decimal, decimal> NoDiscount = total => total;
    public static readonly Func<decimal, decimal> TenPercentDiscount = total => total * 0.9m;
    public static readonly Func<decimal, decimal> TwentyPercentDiscount = total => total * 0.8m;

    public static readonly Predicate<Order> IsLargeOrder = order => order.Total >= 5000m;
}
