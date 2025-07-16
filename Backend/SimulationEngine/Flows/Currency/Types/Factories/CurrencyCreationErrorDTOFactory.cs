using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorDTOFactory : ICurrencyCreationErrorDTOFactory
    {
        private readonly IErrorFactory _errorFactory;
        private readonly ICurrencyCreationDTOFactory _currencyCreationDTOFactory;

        public CurrencyCreationErrorDTOFactory(IErrorFactory errorFactory, ICurrencyCreationDTOFactory currencyCreationDTOFactory)
        {
            _errorFactory = errorFactory;
            _currencyCreationDTOFactory = currencyCreationDTOFactory;
        }
        
        public CurrencyCreationErrorDTO CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            return new CurrencyCreationErrorDTO
            {
                CurrencyCreations = _currencyCreationDTOFactory.CreateFrom(currencyCreations),
                ErrorDetails = _errorFactory.CreateError(exception)
            };
        }
    }
}