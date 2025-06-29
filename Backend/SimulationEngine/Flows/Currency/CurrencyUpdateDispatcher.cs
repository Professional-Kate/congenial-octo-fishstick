using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateDispatcher : ICurrencyUpdateDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        
        public CurrencyUpdateDispatcher(IBufferManager bufferManager, ICurrencyUpdateFactory currencyUpdateFactory)
        {
            _bufferManager = bufferManager;
            _currencyUpdateFactory = currencyUpdateFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyTrade> trades)
        {
            IBuffer<CurrencyUpdateDTO> buffer = _bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(trades.Count));
            buffer.Assign(_currencyUpdateFactory.CreateFrom(trades));
            buffer.MarkReady();
        }
    }
}