using IdelPog.Common.Factories;
using IdelPog.Messaging.Factory;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorDTOFactory: IErrorFactory<CurrencyCreationErrorDTO, CurrencyCreation>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyCreationDTOFactory _currencyCreationDTOFactory;

        public CurrencyCreationErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyCreationDTOFactory currencyCreationDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyCreationDTOFactory = currencyCreationDTOFactory;
        }

        public CurrencyCreationErrorDTO Create(CurrencyCreation context, Exception exception)
        {
            return new CurrencyCreationErrorDTO
            {
                CurrencyCreation = _currencyCreationDTOFactory.CreateFrom(context),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}