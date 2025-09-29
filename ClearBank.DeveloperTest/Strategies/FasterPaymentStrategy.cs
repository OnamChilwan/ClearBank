using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public class FasterPaymentStrategy : IPaymentStrategy
{
    public PaymentScheme SupportedScheme => PaymentScheme.FasterPayments;

    public MakePaymentResult MakePayment(Account account, MakePaymentRequest request)
    {
        var result = new MakePaymentResult
        {
            Success = true
        };
        
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
        {
            result.Success = false;
        }
        else if (account.Balance < request.Amount)
        {
            result.Success = false;
        }
        
        return result;
    }
}