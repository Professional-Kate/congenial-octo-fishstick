using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorDTOFactory : ICurrencyUpdateErrorDTOFactory
    {
        private readonly IErrorFactory _errorFactory;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;

        public CurrencyUpdateErrorDTOFactory(IErrorFactory errorFactory, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory)
        {
            _errorFactory = errorFactory;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
        }
        
        public CurrencyUpdateErrorDTO CreateCurrencyUpdateError(IReadOnlyList<CurrencyUpdate> updates, Exception exception)
        {
            return new CurrencyUpdateErrorDTO
            {
                CurrencyUpdates = _currencyUpdateDTOFactory.CreateFrom(updates),
                ErrorDetails = _errorFactory.CreateError(exception)
            };
        }
    }
}