using IdelPog.Common.Commands;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyUpdateListener : IBufferListener<CurrencyUpdate>
    {
        private readonly ICurrencyController _currencyController;
        private readonly IDispatchOne<CurrencyUpdateErrorDTO> _currencyUpdateErrorDTODispatcher;
        private readonly ICurrencyUpdateErrorDTOFactory _currencyUpdateErrorDTOFactory;

        public CurrencyUpdateListener(ICurrencyController currencyController, IDispatchOne<CurrencyUpdateErrorDTO> currencyUpdateErrorDTODispatcher,
            ICurrencyUpdateErrorDTOFactory currencyUpdateErrorDTOFactory)
        {
            _currencyController = currencyController;
            _currencyUpdateErrorDTODispatcher = currencyUpdateErrorDTODispatcher;
            _currencyUpdateErrorDTOFactory = currencyUpdateErrorDTOFactory;
        }

        public Type ListenerType { get; } = typeof(CurrencyUpdate);

        public void Handle(IReadOnlyList<CurrencyUpdate> buffer)
        {
            try
            {
                _currencyController.UpdateCurrency(buffer);
            }
            catch (Exception exception)
            {
                _currencyUpdateErrorDTODispatcher.Dispatch(_currencyUpdateErrorDTOFactory.CreateCurrencyUpdateError(buffer, exception));
            }
        }
    }
}