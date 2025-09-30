using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public class ChapsPaymentStrategy : IPaymentStrategy
{
    public PaymentScheme SupportedScheme => PaymentScheme.Chaps;
    
    public MakePaymentResult MakePayment(Account account, MakePaymentRequest request)
    {
        var result = MakePaymentResult.Successful();
        
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) || account.Status != AccountStatus.Live)
        {
            result.Success = false;
        }

        return result;
    }
}