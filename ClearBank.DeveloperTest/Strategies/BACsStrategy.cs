using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Strategies;

public class BACsStrategy
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