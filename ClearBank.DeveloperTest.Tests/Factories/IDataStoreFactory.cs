using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Tests.Factories;

public interface IDataStoreFactory
{
    IAccountDataStore Get(string dataStoreType);
}