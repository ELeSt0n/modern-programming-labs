namespace OnlineStore.Domain;

public enum OrderStatus
{
    Created,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}

public enum OrderCategory
{
    Small,
    Medium,
    Large
}
