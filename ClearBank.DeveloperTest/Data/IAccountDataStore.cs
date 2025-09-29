using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public interface IAccountDataStore
{
    DataStoreType SupportedDataStoreType { get; }
    
    Account GetAccount(string accountNumber);

    void UpdateAccount(Account account);
}