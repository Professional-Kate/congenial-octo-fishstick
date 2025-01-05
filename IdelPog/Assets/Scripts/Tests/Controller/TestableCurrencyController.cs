using IdelPog.Controller.Currency;
using IdelPog.Service.Currency;

namespace Tests.Controller
{
    /// <summary>
    /// Just for testing. The purpose of this class is to act as a dependency injector to enable mocking within tests.
    /// </summary>
    internal class TestableCurrencyController : CurrencyController
    {
        internal TestableCurrencyController(ICurrencyMediator currencyService)
        {
            CurrencyService = currencyService;
        }   
    }
}