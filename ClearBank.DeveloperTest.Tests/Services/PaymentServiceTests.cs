using System.Collections;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    private const string DataStoreType = "data_store";
    private readonly IAccountDataStore _accountStore = Substitute.For<IAccountDataStore>();
    private readonly IDataStoreFactory _dataStoreFactory = Substitute.For<IDataStoreFactory>();
    private readonly IPaymentStrategyFactory _paymentStrategyFactory = Substitute.For<IPaymentStrategyFactory>();
    private PaymentService _subject;

    [SetUp]
    public void Setup()
    {
        _dataStoreFactory
            .Get(DataStoreType)
            .Returns(_accountStore);
        
        _subject = new PaymentService(
            new PaymentConfiguration { DataStoreType = DataStoreType },
            _dataStoreFactory,
            _paymentStrategyFactory);
    }

    [TestCase(PaymentScheme.Bacs)]
    [TestCase(PaymentScheme.Chaps)]
    [TestCase(PaymentScheme.FasterPayments)]
    public void Given_Account_Does_Not_Exist_Then_Unsuccessful_Result_Is_Returned(PaymentScheme paymentScheme)
    {
        var request = new MakePaymentRequest { PaymentScheme = paymentScheme };
        var result = _subject.MakePayment(request);
        result.Success.Should().BeFalse();
    }

    [TestCaseSource(typeof(TestCases))]
    public void Given_Account_Exists_When_Making_Unsuccessful_Payment_Then_Correct_Payment_Strategy_Is_Used(Account account, PaymentScheme scheme)
    {
        var request = new MakePaymentRequest { DebtorAccountNumber = "123", PaymentScheme = scheme };
        var strategy = Substitute.For<IPaymentStrategy>();

        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);

        _paymentStrategyFactory
            .Get(request.PaymentScheme)
            .Returns(strategy);

        strategy
            .MakePayment(account, request)
            .Returns(MakePaymentResult.Unsuccessful());

        var result = _subject.MakePayment(request);
        
        result.Success.Should().BeFalse();
        
        strategy.Received(1).MakePayment(account, request);
    }
    
    [TestCaseSource(typeof(TestCases))]
    public void Given_Account_Exists_When_Making_Successful_Payment_Then_Correct_Payment_Strategy_Is_Used_And_Account_Balance_Is_Updated(Account account, PaymentScheme scheme)
    {
        var request = new MakePaymentRequest { DebtorAccountNumber = "123", PaymentScheme = scheme, Amount = 50.5M };
        var strategy = Substitute.For<IPaymentStrategy>();

        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);

        _paymentStrategyFactory
            .Get(request.PaymentScheme)
            .Returns(strategy);
        
        strategy
            .MakePayment(account, request)
            .Returns(MakePaymentResult.Successful());

        var result = _subject.MakePayment(request);
        
        result.Success.Should().BeTrue();
        
        strategy.Received(1).MakePayment(account, request);
        
        _accountStore
            .Received(1)
            .UpdateAccount(Arg.Is<Account>(x =>
                x.AllowedPaymentSchemes == account.AllowedPaymentSchemes && 
                x.Balance == 49.50M &&
                x.Status == account.Status &&
                x.AccountNumber == account.AccountNumber));
    }
    
    public class TestCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[]
            {
                new Account { Balance = 100, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, AccountNumber = "123", Status = AccountStatus.Live }, 
                PaymentScheme.Bacs
            };
            yield return new object[]
            {
                new Account { Balance = 100, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, AccountNumber = "123", Status = AccountStatus.InboundPaymentsOnly }, 
                PaymentScheme.FasterPayments
            };
            yield return new object[]
            {
                new Account { Balance = 100, AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, AccountNumber = "123", Status = AccountStatus.Disabled }, 
                PaymentScheme.Chaps
            };
        }
    }
}