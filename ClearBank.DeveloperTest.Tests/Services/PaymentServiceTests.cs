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

    [Test]
    public void Given_Account_Does_Exists_When_Making_BACs_Payment_Then_Unsuccessful_Response_Is_Returned()
    {
        var request = new MakePaymentRequest { DebtorAccountNumber = "123", PaymentScheme = PaymentScheme.Bacs };
        var result = _subject.MakePayment(request);

        _factory.Received(1).Get("data_store");
        _accountStore.Received(1).GetAccount(request.DebtorAccountNumber);
    
        result.Success.Should().BeFalse();
    }
}