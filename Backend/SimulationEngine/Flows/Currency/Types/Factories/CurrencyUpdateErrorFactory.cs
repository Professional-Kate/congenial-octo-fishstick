using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorFactory : ICurrencyUpdateErrorFactory
    {
        private readonly IErrorFactory _errorFactory;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;

        public CurrencyUpdateErrorFactory(IErrorFactory errorFactory, ICurrencyUpdateFactory currencyUpdateFactory)
        {
            _errorFactory = errorFactory;
            _currencyUpdateFactory = currencyUpdateFactory;
        }
        
        public CurrencyUpdateErrorDTO CreateCurrencyUpdateError(IReadOnlyList<CurrencyUpdate> updates, Exception exception)
        {
            return new CurrencyUpdateErrorDTO
            {
                CurrencyUpdates = _currencyUpdateFactory.CreateFrom(updates),
                ErrorDetails = _errorFactory.CreateError(exception)
            };
        }
    }
}