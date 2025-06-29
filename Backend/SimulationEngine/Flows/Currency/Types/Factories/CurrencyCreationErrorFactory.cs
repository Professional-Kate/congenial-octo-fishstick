using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorFactory : ICurrencyCreationErrorFactory
    {
        private readonly IErrorFactory _errorFactory;
        private readonly ICurrencyCreationFactory _currencyCreationFactory;

        public CurrencyCreationErrorFactory(IErrorFactory errorFactory, ICurrencyCreationFactory currencyCreationFactory)
        {
            _errorFactory = errorFactory;
            _currencyCreationFactory = currencyCreationFactory;
        }
        
        public CurrencyCreationErrorDTO CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            return new CurrencyCreationErrorDTO
            {
                CurrencyCreations = _currencyCreationFactory.CreateFrom(currencyCreations),
                ErrorDetails = _errorFactory.CreateError(exception)
            };
        }
    }
}