namespace OnlineStore.Domain;

/// <summary>Value object: дві адреси з однаковими полями вважаються рівними.</summary>
public sealed record Address(
    string City,
    string Street,
    string Building,
    string PostalCode);
