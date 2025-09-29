using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    private readonly PaymentService _subject;

    public PaymentServiceTests()
    {
        const string dataStoreType = "data_store";

        _subject = new PaymentService(
            new PaymentConfiguration { DataStoreType = dataStoreType });
    }
    
    [Test]
    public void Given_Account_Does_Exists_When_Making_BACs_Payment_Then_Unsuccessful_Response_Is_Returned()
    {
        var request = new MakePaymentRequest { DebtorAccountNumber = "123", PaymentScheme = PaymentScheme.Bacs };
        var result = _subject.MakePayment(request);
    
        result.Success.Should().BeFalse();
    }
}