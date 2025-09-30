using System;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Factories;

public class PaymentStrategyFactoryTests
{
    private readonly IPaymentStrategy _strategyA = Substitute.For<IPaymentStrategy>();
    private readonly IPaymentStrategy _strategyB = Substitute.For<IPaymentStrategy>();
    private PaymentStrategyFactory _subject;

    [SetUp]
    public void Setup()
    {
        _strategyA.SupportedScheme.Returns(PaymentScheme.Bacs);
        _strategyB.SupportedScheme.Returns(PaymentScheme.Chaps);
        _subject = new PaymentStrategyFactory(new[]
        {
            _strategyA,
            _strategyB
        });    
    }

    [Test]
    public void Given_Strategy_Is_Registered_When_Retrieving_Then_Strategy_Is_Returned()
    {
        var result = _subject.Get(PaymentScheme.Bacs);
        
        result.Should().Be(_strategyA);
    }

    [Test]
    public void Given_Strategy_Is_Not_Registered_When_Retrieving_Then_Exception_Is_Thrown()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _subject.Get(PaymentScheme.FasterPayments));
        exception!.Message.Should().Be("No strategy found for payment scheme: FasterPayments");       
    }

    [Test]
    public void Given_Multiple_Strategies_Are_Registered_For_Same_Scheme_Then_Exception_Is_Thrown()
    {
        _strategyA.SupportedScheme.Returns(PaymentScheme.Bacs);
        _strategyB.SupportedScheme.Returns(PaymentScheme.Bacs);

        Assert.Throws<InvalidOperationException>(() => _subject.Get(PaymentScheme.Bacs));
    }
}