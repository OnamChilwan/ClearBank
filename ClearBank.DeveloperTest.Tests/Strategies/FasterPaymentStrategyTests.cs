using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Strategies;

public class FasterPaymentStrategyTests
{
    private FasterPaymentStrategy _subject = new();

    [SetUp]
    public void Setup()
    {
        _subject = new FasterPaymentStrategy();
    }
    
    [Test]
    public void Strategy_Should_Support_FasterPayments()
    {
        _subject.SupportedScheme.Should().Be(PaymentScheme.FasterPayments);
    }
    
    [Test]
    public void Given_Account_Supports_FasterPayments_And_Has_Sufficient_Funds_Then_Successful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 100m
        };
        var request = new MakePaymentRequest { Amount = 50m };

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeTrue();
    }

    [Test]
    public void Given_Account_Supports_FasterPayments_And_Balance_Equals_Amount_Then_Successful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 100m
        };
        var request = new MakePaymentRequest { Amount = 100m };

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeTrue();
    }

    [Test]
    public void Given_Account_Does_Not_Support_FasterPayments_Then_Unsuccessful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 100m
        };
        var request = new MakePaymentRequest { Amount = 10m };

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeFalse();
    }

    [Test]
    public void Given_Account_Supports_FasterPayments_But_Has_Insufficient_Funds_Then_Unsuccessful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 99.99m
        };
        var request = new MakePaymentRequest { Amount = 100m };

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeFalse();
    }
}