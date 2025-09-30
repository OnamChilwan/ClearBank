using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Strategies;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentConfiguration _paymentConfiguration;
    private readonly IDataStoreFactory _dataStoreFactory;
    private readonly IPaymentStrategyFactory _paymentStrategyFactory;

    public PaymentService(
        PaymentConfiguration paymentConfiguration,
        IDataStoreFactory dataStoreFactory,
        IPaymentStrategyFactory paymentStrategyFactory)
    {
        _paymentConfiguration = paymentConfiguration;
        _dataStoreFactory = dataStoreFactory;
        _paymentStrategyFactory = paymentStrategyFactory;
    }

    public PaymentService() // Maintains backwards compatibility
    {
        _paymentConfiguration = new PaymentConfiguration
        {
            DataStoreType = ConfigurationManager.AppSettings["DataStoreType"]
        };
        
        _dataStoreFactory = new DataStoreFactory(new List<IAccountDataStore>
        {
            new AccountDataStore(), 
            new BackupAccountDataStore()
        });
        
        _paymentStrategyFactory = new PaymentStrategyFactory(new List<IPaymentStrategy>
        {
            new BACsStrategy(),
            new FasterPaymentStrategy(),
            new ChapsPaymentStrategy()
        });
    }
    
    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        var dataStoreType = _paymentConfiguration.DataStoreType;
        var dataStore = _dataStoreFactory.Get(dataStoreType);

        Account account = dataStore.GetAccount(request.DebtorAccountNumber);

        if (account == null)
        {
            return MakePaymentResult.Unsuccessful();
        }

        var strategy = _paymentStrategyFactory.Get(request.PaymentScheme);
        var result = strategy.MakePayment(account, request);

        if (result.Success)
        {
            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);
        }

        return result;
    }
}