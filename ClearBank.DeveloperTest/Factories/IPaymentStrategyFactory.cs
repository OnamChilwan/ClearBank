using ClearBank.DeveloperTest.Strategies;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Factories;

public interface IPaymentStrategyFactory
{
    IPaymentStrategy Get(PaymentScheme scheme);
}