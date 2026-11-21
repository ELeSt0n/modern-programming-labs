namespace OnlineStore.Domain.Payments;

public sealed class CardPayment : IPaymentMethod
{
    private readonly string _lastFourDigits;

    public CardPayment(string lastFourDigits)
    {
        if (lastFourDigits is not { Length: 4 } || !lastFourDigits.All(char.IsAsciiDigit))
            throw new ArgumentException("Card suffix must contain 4 digits.", nameof(lastFourDigits));

        _lastFourDigits = lastFourDigits;
    }

    public string Name => "Card";

    public string Pay(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        return $"Card **** {_lastFourDigits}: charged {amount:F2} UAH";
    }
}
