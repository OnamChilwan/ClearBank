using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Tests.Factories;

public class DataStoreFactory(IEnumerable<IAccountDataStore> accountDataStores) : IDataStoreFactory
{
    public IAccountDataStore Get(string dataStoreType)
    {
        return dataStoreType.Equals("backup", StringComparison.InvariantCultureIgnoreCase) 
            ? accountDataStores.Single(x => x.SupportedDataStoreType == DataStoreType.Backup) 
            : accountDataStores.Single(x => x.SupportedDataStoreType == DataStoreType.Account);
    }
}