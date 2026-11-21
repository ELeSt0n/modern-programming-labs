namespace OnlineStore.Domain.Payments;

public sealed class BankTransferPayment : IPaymentMethod
{
    private readonly string _invoiceNumber;

    public BankTransferPayment(string invoiceNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(invoiceNumber);
        _invoiceNumber = invoiceNumber;
    }

    public string Name => "Bank transfer";

    public string Pay(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        return $"Bank transfer by invoice {_invoiceNumber}: {amount:F2} UAH";
    }
}
