using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyCreationErrorDispatcher : ICurrencyCreationErrorDispatcher
    {
        private readonly IErrorFactory _errorFactory;

        public CurrencyCreationErrorDispatcher(IErrorFactory errorFactory)
        {
            _errorFactory = errorFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}