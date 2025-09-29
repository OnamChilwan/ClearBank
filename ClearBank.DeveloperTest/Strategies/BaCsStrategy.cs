using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public class BaCsStrategy
{
    public MakePaymentResult MakePayment(Account account, MakePaymentRequest request)
    {
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
        {
            return new MakePaymentResult();
        }

        return new MakePaymentResult { Success = true };
    }
}