using ClearBank.DeveloperTest.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Data;

public class AccountDataStoreTests
{
    [Test]
    public void Supported_Data_Stored_Type_Should_Be_Account()
    {
        var subject = new AccountDataStore();
        subject.SupportedDataStoreType.Should().Be(DataStoreType.Account);
    }
}