using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Types;

public class MakePaymentResultTests
{
    [Test]
    public void Successful_Should_Return_Result_With_Success_True()
    {
        var result = MakePaymentResult.Successful();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Test]
    public void Unsuccessful_Should_Return_Result_With_Success_False()
    {
        var result = MakePaymentResult.Unsuccessful();
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }
}