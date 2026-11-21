namespace OnlineStore.Domain;

public sealed class Product
{
    public Product(string name, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Price = price;
    }

    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }
}
