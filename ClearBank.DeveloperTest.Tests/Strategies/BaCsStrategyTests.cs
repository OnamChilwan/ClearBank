using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Strategies;

public class BaCsStrategyTests
{
    private readonly BACsStrategy _subject = new();

    [Test]
    public void Strategy_Should_Support_Bacs()
    {
        _subject.SupportedScheme.Should().Be(PaymentScheme.Bacs); 
    }
    
    [Test]
    public void Given_Account_Supports_BaCs_Then_Successful_Result_Is_Returned()
    {
        var result = _subject.MakePayment(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs }, new MakePaymentRequest());

        result.Success.Should().BeTrue();
    }
    
    [TestCase(AllowedPaymentSchemes.Chaps)]
    [TestCase(AllowedPaymentSchemes.FasterPayments)]
    public void Given_Account_Supports_BaCs_Then_Successful_Result_Is_Returned(AllowedPaymentSchemes scheme)
    {
        var result = _subject.MakePayment(new Account { AllowedPaymentSchemes = scheme }, new MakePaymentRequest());

        result.Success.Should().BeFalse();
    }
}