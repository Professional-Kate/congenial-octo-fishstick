using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorDTOFactory : IErrorFactory<CurrencyUpdateErrorDTO, IReadOnlyList<CurrencyUpdate>>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;

        public CurrencyUpdateErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
        }

        public CurrencyUpdateErrorDTO Create<TException>(IReadOnlyList<CurrencyUpdate> context, TException exception) where TException : Exception
        {
            return new CurrencyUpdateErrorDTO
            {
                CurrencyUpdates = _currencyUpdateDTOFactory.CreateFrom(context),
                ErrorDetails = _errorDTOFactory.Create(exception)
            };
        }
    }
}