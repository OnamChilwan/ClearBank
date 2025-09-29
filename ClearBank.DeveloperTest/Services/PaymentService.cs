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

    public PaymentService(
        PaymentConfiguration paymentConfiguration,
        IDataStoreFactory dataStoreFactory)
    {
        _paymentConfiguration = paymentConfiguration;
        _dataStoreFactory = dataStoreFactory;
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

        var result = MakePaymentResult.Successful();
            
        switch (request.PaymentScheme) // TODO: Factory create specific type (strategy)
        {
            case PaymentScheme.Bacs:
                result = new BACsStrategy().MakePayment(account, request);
                break;

            case PaymentScheme.FasterPayments:
                if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                {
                    result.Success = false;
                }
                else if (account.Balance < request.Amount)
                {
                    result.Success = false;
                }
                break;

            case PaymentScheme.Chaps:
                if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                {
                    result.Success = false;
                }
                else if (account.Status != AccountStatus.Live)
                {
                    result.Success = false;
                }
                break;
        }

        if (result.Success)
        {
            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);
        }

        return result;
    }
}