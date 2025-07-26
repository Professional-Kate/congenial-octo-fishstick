using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyCreationListener : IBufferListener<CurrencyCreation>
    {
        private readonly ICurrencyController _currencyController;
        private readonly IDispatchOne<CurrencyCreationErrorDTO> _currencyCreationErrorDTODispatcher;
        private readonly ICurrencyCreationErrorDTOFactory _currencyCreationErrorDTOFactory;

        public CurrencyCreationListener(ICurrencyController currencyController, IDispatchOne<CurrencyCreationErrorDTO> currencyCreationErrorDTODispatcher,
            ICurrencyCreationErrorDTOFactory currencyCreationErrorDTOFactory)
        {
            _currencyController = currencyController;
            _currencyCreationErrorDTODispatcher = currencyCreationErrorDTODispatcher;
            _currencyCreationErrorDTOFactory = currencyCreationErrorDTOFactory;
        }

        public Type ListenerType { get; } = typeof(CurrencyCreation);

        public void Handle(IReadOnlyList<CurrencyCreation> buffer)
        {
            try
            {
                _currencyController.CreateCurrency(buffer);
            }
            catch (Exception exception)
            {
                _currencyCreationErrorDTODispatcher.Dispatch(_currencyCreationErrorDTOFactory.CreateCurrencyCreationError(buffer, exception));
            }
        }
    }
}