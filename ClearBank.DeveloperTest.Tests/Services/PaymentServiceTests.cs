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
    private readonly PaymentService _subject;
    private readonly IAccountDataStore _accountStore = Substitute.For<IAccountDataStore>();
    private readonly IDataStoreFactory _dataStoreFactory = Substitute.For<IDataStoreFactory>();
    private readonly IPaymentStrategyFactory _paymentStrategyFactory = Substitute.For<IPaymentStrategyFactory>();

    public PaymentServiceTests()
    {
        const string dataStoreType = "data_store";
        
        _dataStoreFactory
            .Get(dataStoreType)
            .Returns(_accountStore);

        _subject = new PaymentService(
            new PaymentConfiguration { DataStoreType = dataStoreType },
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
    public void Given_Account_Exists_When_Making_Payment_Then_Correct_Payment_Strategy_Is_Used(Account account, PaymentScheme scheme, bool expectedResult)
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
    
    public class TestCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new Account(), PaymentScheme.Bacs, false };
            yield return new object[] { new Account(), PaymentScheme.FasterPayments, false };
            yield return new object[] { new Account(), PaymentScheme.Chaps, false };
        }
    }
}