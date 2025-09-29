using ClearBank.DeveloperTest.ComponentTests.Steps;
using ClearBank.DeveloperTest.Types;
using TestStack.BDDfy;

namespace ClearBank.DeveloperTest.ComponentTests.Fixtures;

public class PaymentTests
{
    private readonly PaymentSteps _steps = new();
    
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.Bacs)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.FasterPayments)]
    public void Successful_Payment(PaymentScheme scheme, AllowedPaymentSchemes allowedScheme)
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1000,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = allowedScheme
        };

        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = scheme,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsSuccessful())
            .BDDfy();
    }

    [TestCase(PaymentScheme.Chaps)]
    [TestCase(PaymentScheme.Bacs)]
    [TestCase(PaymentScheme.FasterPayments)]
    public void Unsuccessful_Payment_When_Account_Does_Not_Exist(PaymentScheme scheme)
    {
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = "123",
            PaymentScheme = scheme,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };
        
        this.Given(s => _steps.AccountDoesNotExist())
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful())
            .BDDfy();
    }

    [Test]
    public void Unsuccessful_BACs_Payment_When_Account_Does_Not_Support_Payment_Scheme()
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1000,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };
        
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = PaymentScheme.Chaps,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful());
    }

    [Test]
    public void Unsuccessful_Faster_Payment_When_Account_Does_Not_Support_Payment_Scheme()
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1000,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
        };
        
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = PaymentScheme.Chaps,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful());
    }

    [Test]
    public void Unsuccessful_Faster_Payment_Insufficient_Balance()
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
        };
        
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = PaymentScheme.FasterPayments,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful());
    }
    
    [Test]
    public void Unsuccessful_Chaps_Payment_When_Account_Does_Not_Support_Payment_Scheme()
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1000,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };
        
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = PaymentScheme.Bacs,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful());
    }

    [TestCase(AccountStatus.Disabled)]
    [TestCase(AccountStatus.InboundPaymentsOnly)]
    public void Unsuccessful_Chaps_Payment_When_Account_Is_Not_Live(AccountStatus status)
    {
        var account = new Account
        {
            AccountNumber = "1234567890",
            Balance = 1000,
            Status = status,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };
        
        var request = new MakePaymentRequest
        {
            Amount = 100,
            DebtorAccountNumber = account.AccountNumber,
            PaymentScheme = PaymentScheme.Chaps,
            CreditorAccountNumber = "999",
            PaymentDate = DateTime.UtcNow
        };

        this.Given(s => _steps.AccountExists(account))
            .When(s => _steps.MakingPayment(request))
            .Then(s => _steps.PaymentIsUnsuccessful());
    }
}