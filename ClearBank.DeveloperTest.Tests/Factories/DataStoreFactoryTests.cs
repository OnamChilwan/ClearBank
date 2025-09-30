using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Factories;

public class DataStoreFactoryTests
{
    private readonly IAccountDataStore _accountDataStore = Substitute.For<IAccountDataStore>();
    private readonly IAccountDataStore _backupAccountDataStore = Substitute.For<IAccountDataStore>();
    private DataStoreFactory _subject;

    public DataStoreFactoryTests()
    {
        _backupAccountDataStore
            .SupportedDataStoreType
            .Returns(DataStoreType.Backup);
        
        _accountDataStore
            .SupportedDataStoreType
            .Returns(DataStoreType.Account);
    }

    [SetUp]
    public void Setup()
    {
        _subject = new DataStoreFactory([_accountDataStore, _backupAccountDataStore]); 
    }

    [TestCase("Backup")]
    [TestCase("backup")]
    [TestCase("BACKUP")]
    public void Give_Data_Store_Type_Is_Backup_When_Retrieving_Account_Store_Then_Correct_Instance_Is_Returned(string dataStoreType)
    {
        var result = _subject.Get(dataStoreType);
        result.Should().Be(_backupAccountDataStore);
    }
    
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("foo")]
    public void Give_Data_Store_Type_Is_Other_When_Retrieving_Account_Store_Then_Correct_Instance_Is_Returned(string dataStoreType)
    {
        var result = _subject.Get(dataStoreType);
        result.Should().Be(_accountDataStore);
    }
}