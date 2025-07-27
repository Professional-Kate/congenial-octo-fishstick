using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Factory;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorDTOFactory : IErrorFactory<CurrencyUpdateErrorDTO, CurrencyUpdate>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;

        public CurrencyUpdateErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
        }
        
        public CurrencyUpdateErrorDTO Create(CurrencyUpdate context, Exception exception)
        {
            return new CurrencyUpdateErrorDTO
            {
                CurrencyUpdate = _currencyUpdateDTOFactory.CreateFrom(context),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}