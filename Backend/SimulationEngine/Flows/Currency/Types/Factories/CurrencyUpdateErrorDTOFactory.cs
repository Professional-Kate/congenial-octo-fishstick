using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorDTOFactory : ICurrencyUpdateErrorDTOFactory
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;

        public CurrencyUpdateErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
        }
        
        public CurrencyUpdateErrorDTO CreateCurrencyUpdateError(IReadOnlyList<CurrencyUpdate> updates, Exception exception)
        {
            return new CurrencyUpdateErrorDTO
            {
                CurrencyUpdates = _currencyUpdateDTOFactory.CreateFrom(updates),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}