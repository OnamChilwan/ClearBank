using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;

namespace ClearBank.DeveloperTest.ComponentTests.Steps;

public class PaymentSteps
{
    private readonly PaymentService _paymentService;
    private readonly IAccountDataStore _accountDataStore = Substitute.For<IAccountDataStore>();
    private MakePaymentResult? _result;

    public PaymentSteps()
    {
        _accountDataStore
            .SupportedDataStoreType
            .Returns(DataStoreType.Account);
        
        _paymentService = new PaymentService(
            new PaymentConfiguration { DataStoreType = string.Empty },
            new DataStoreFactory([ _accountDataStore ]), 
            new PaymentStrategyFactory(new List<IPaymentStrategy>
            {
                new BACsStrategy(),
                new FasterPaymentStrategy(),
                new ChapsPaymentStrategy()
            }));
    }
    
    public void AccountExists(Account account)
    {
        _accountDataStore
            .GetAccount(account.AccountNumber)
            .Returns(account);
    }

    public void AccountDoesNotExist()
    {
        _accountDataStore
            .GetAccount(Arg.Any<string>())
            .Returns((Account?)null); 
    }

    public void MakingPayment(MakePaymentRequest request)
    {
        _result = _paymentService.MakePayment(request);
    }

    public void PaymentIsSuccessful()
    {
        _result!.Success.Should().BeTrue();
    }

    public void PaymentIsUnsuccessful()
    {
        _result!.Success.Should().BeFalse();
    }
}