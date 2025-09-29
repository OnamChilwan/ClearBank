using ClearBank.DeveloperTest.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Data;

public class BackupAccountDataStoreTests
{
    [Test]
    public void Supported_Data_Stored_Type_Should_Be_Backup()
    {
        var subject = new BackupAccountDataStore();
        subject.SupportedDataStoreType.Should().Be(DataStoreType.Backup);
    }
}