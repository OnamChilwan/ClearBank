using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Strategies;

public class ChapsPaymentStrategyTests
{
    private readonly ChapsPaymentStrategy _subject = new();

    [Test]
    public void Strategy_Should_Support_Chaps()
    {
        _subject.SupportedScheme.Should().Be(PaymentScheme.Chaps);
    }

    [Test]
    public void Given_Account_Supports_Chaps_And_Is_Live_Then_Successful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
        };
        var request = new MakePaymentRequest();

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeTrue();
    }

    [Test]
    public void Given_Account_Does_Not_Support_Chaps_Then_Unsuccessful_Result_Is_Returned()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
        };
        var request = new MakePaymentRequest();

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeFalse();
    }

    [TestCase(AccountStatus.Disabled)]
    [TestCase(AccountStatus.InboundPaymentsOnly)]
    public void Given_Account_Supports_Chaps_But_Is_Not_Live_Then_Unsuccessful_Result_Is_Returned(AccountStatus status)
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = status
        };
        var request = new MakePaymentRequest();

        var result = _subject.MakePayment(account, request);

        result.Success.Should().BeFalse();
    }
}