using System.Collections;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    private readonly PaymentService _subject;
    private readonly IAccountDataStore _accountStore = Substitute.For<IAccountDataStore>();
    private readonly IDataStoreFactory _factory = Substitute.For<IDataStoreFactory>();

    public PaymentServiceTests()
    {
        const string dataStoreType = "data_store";
        
        _factory
            .Get(dataStoreType)
            .Returns(_accountStore);

        _subject = new PaymentService(
            new PaymentConfiguration { DataStoreType = dataStoreType },
            _factory);
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
    public void Given_Account_Exists_But_Account_Does_Not_Support_Payment_Scheme_Then_Unsuccessful_Result_Is_Returned(Account account, PaymentScheme scheme, bool expectedResult)
    {
        var request = new MakePaymentRequest { DebtorAccountNumber = "123", PaymentScheme = scheme };

        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);

        var result = _subject.MakePayment(request);
        
        result.Success.Should().BeFalse();
    }

    [Test]
    public void Given_Faster_Payment_Account_With_Insufficient_Funds_Then_Unsuccessful_Result_Is_Returned()
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "123", 
            PaymentScheme = PaymentScheme.FasterPayments, 
            Amount = 100.01M
        };
        
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 100
        };
        
        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);
        
        var result = _subject.MakePayment(request);
        
        result.Success.Should().BeFalse();
    }

    [TestCase(AccountStatus.Disabled)]
    [TestCase(AccountStatus.InboundPaymentsOnly)]
    public void Given_Non_Live_Chaps_Account_When_Making_Payment_Then_Unsuccessful_Result_Is_Returned(AccountStatus accountStatus)
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "123", 
            PaymentScheme = PaymentScheme.Chaps
        };
        
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = accountStatus
        };
        
        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);
        
        var result = _subject.MakePayment(request);
        
        result.Success.Should().BeFalse();
    }

    [TestCase(PaymentScheme.FasterPayments,AllowedPaymentSchemes.FasterPayments)]
    [TestCase(PaymentScheme.Bacs,AllowedPaymentSchemes.Bacs)]
    [TestCase(PaymentScheme.Chaps,AllowedPaymentSchemes.Chaps)]
    public void Given_Account_Exists_With_Sufficient_Funds_Then_Successful_Result_Is_Returned(PaymentScheme scheme, AllowedPaymentSchemes allowedScheme)
    {
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "123", 
            PaymentScheme = scheme,
            Amount = 50.50M
        };

        var account = new Account
        {
            AllowedPaymentSchemes = allowedScheme,
            Balance = 100,
            Status = AccountStatus.Live
        };
        
        _accountStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns(account);
        
        var result = _subject.MakePayment(request);
        result.Success.Should().BeTrue();
        
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
            yield return new object[] { new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps }, PaymentScheme.Bacs, false };
            yield return new object[] { new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps }, PaymentScheme.FasterPayments, false };
            yield return new object[] { new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs }, PaymentScheme.Chaps, false };
        }
    }
}