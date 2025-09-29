namespace ClearBank.DeveloperTest.Types;

public class MakePaymentResult
{
    public bool Success { get; set; }
    
    public static MakePaymentResult Unsuccessful()
    {
        return new MakePaymentResult { Success = false };
    }
    
    public static MakePaymentResult Successful()
    {
        return new MakePaymentResult { Success = true };
    }
}