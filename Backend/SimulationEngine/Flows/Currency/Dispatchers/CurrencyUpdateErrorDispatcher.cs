using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyUpdateErrorDispatcher : ICurrencyUpdateErrorDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyUpdateErrorFactory _currencyUpdateErrorFactory;

        public CurrencyUpdateErrorDispatcher(IBufferManager bufferManager, ICurrencyUpdateErrorFactory currencyUpdateErrorFactory)
        {
            _bufferManager = bufferManager;
            _currencyUpdateErrorFactory = currencyUpdateErrorFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyUpdate> updates, Exception exception)
        {
            IBuffer<CurrencyUpdateErrorDTO> buffer = _bufferManager.RequestBuffer<CurrencyUpdateErrorDTO>(new BufferRequest(1));
            buffer.Assign([_currencyUpdateErrorFactory.CreateCurrencyUpdateError(updates, exception)]);
            buffer.MarkReady();
        }
    }
}