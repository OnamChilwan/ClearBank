using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public interface IPaymentStrategy
{
    PaymentScheme SupportedScheme { get; }
    
    MakePaymentResult MakePayment(Account account, MakePaymentRequest request);
}