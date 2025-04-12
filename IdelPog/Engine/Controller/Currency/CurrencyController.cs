using IdelPog.Engine.Orchestration.Currency;
using IdelPog.Engine.Structures;

namespace IdelPog.Engine.Controller.Currency
{
    /// <summary>
    /// The main control object for Currency. Using this class you can Update any Currency.
    /// </summary>
    /// <seealso cref="UpdateCurrency"/>
    public class CurrencyController : ICurrencyController
    {
        private readonly ICurrencyMediator _currencyService;

        public CurrencyController(ICurrencyMediator currencyService)
        {
            _currencyService = currencyService;
        }
        
        public ServiceResponse UpdateCurrency(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            if (serviceResponse.IsSuccess == false)
            {
                // TODO: logger log.
                Console.WriteLine(serviceResponse.Message);
            }
            
            return serviceResponse;
        }
    }
} 