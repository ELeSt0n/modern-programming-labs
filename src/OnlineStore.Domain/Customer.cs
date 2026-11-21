namespace OnlineStore.Domain;

public sealed class Customer
{
    public Customer(string name, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
}
