using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public class BacsStrategy : IPaymentStrategy
{
    public PaymentScheme SupportedScheme => PaymentScheme.Bacs;

    public MakePaymentResult MakePayment(Account account, MakePaymentRequest request)
    {
        return !account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs) 
            ? new MakePaymentResult() 
            : new MakePaymentResult { Success = true };
    }
}