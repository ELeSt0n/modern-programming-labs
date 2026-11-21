namespace OnlineStore.Domain.Payments;

public interface IPaymentMethod
{
    string Name { get; }

    /// <summary>Проводить оплату й повертає короткий опис операції.</summary>
    string Pay(decimal amount);
}
