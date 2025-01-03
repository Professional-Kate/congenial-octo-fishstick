using IdelPog.Repository;
using IdelPog.Repository.Currency;
using IdelPog.Service.Currency;

namespace Tests.Service
{
    /// <summary>
    /// Just for testing. The purpose of this class is to act as a dependency injector to enable mocking within tests.
    /// </summary>
    internal class TestableCurrencyService : CurrencyService
    {
        internal TestableCurrencyService(ICurrencyRepository currencyRepository)
        {
            Repository = currencyRepository;
        }
    }
}