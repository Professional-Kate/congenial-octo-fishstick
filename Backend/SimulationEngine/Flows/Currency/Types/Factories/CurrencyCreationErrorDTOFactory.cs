using IdelPog.Common.DTO.Factories;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorDTOFactory : ICurrencyCreationErrorDTOFactory
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyCreationDTOFactory _currencyCreationDTOFactory;

        public CurrencyCreationErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyCreationDTOFactory currencyCreationDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyCreationDTOFactory = currencyCreationDTOFactory;
        }

        public CurrencyCreationErrorDTO CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            return new CurrencyCreationErrorDTO
            {
                CurrencyCreations = _currencyCreationDTOFactory.CreateFrom(currencyCreations),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}