using IdelPog.Common.Factories;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorDTOFactory: IErrorFactory<CurrencyCreationErrorDTO, IReadOnlyList<CurrencyCreation>>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyCreationDTOFactory _currencyCreationDTOFactory;

        public CurrencyCreationErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyCreationDTOFactory currencyCreationDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyCreationDTOFactory = currencyCreationDTOFactory;
        }

        public CurrencyCreationErrorDTO Create<TException>(IReadOnlyList<CurrencyCreation> context, TException exception) where TException : Exception
        {
            return new CurrencyCreationErrorDTO
            {
                CurrencyCreations = _currencyCreationDTOFactory.CreateFrom(context),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}