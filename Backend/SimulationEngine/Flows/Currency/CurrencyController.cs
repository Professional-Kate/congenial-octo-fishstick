using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    /// <summary>
    /// The main control object for Currency. Using this class you can Update any Currency.
    /// </summary>
    /// <seealso cref="UpdateCurrency"/>
    public class CurrencyController(ICurrencyMediator currencyService) : ICurrencyController
    {
        public ServiceResponse UpdateCurrency(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = currencyService.ProcessCurrencyUpdate(trades);
            if (serviceResponse.IsSuccess == false)
            {
                // TODO: logger log.
                Console.WriteLine(serviceResponse.Message);
            }
            
            return serviceResponse;
        }
    }
} 