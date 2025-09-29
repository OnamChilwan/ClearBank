using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Factories;

public class PaymentStrategyFactory(IEnumerable<IPaymentStrategy> paymentStrategies) : IPaymentStrategyFactory
{
    public IPaymentStrategy Get(PaymentScheme scheme)
    {
        var strategy = paymentStrategies.FirstOrDefault(x => x.SupportedScheme == scheme);
        return strategy ?? throw new NotSupportedException($"No strategy found for payment scheme: {scheme}");
    }
}