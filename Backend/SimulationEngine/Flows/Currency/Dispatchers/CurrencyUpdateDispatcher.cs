using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyUpdateDispatcher : ICurrencyUpdateDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;
        
        public CurrencyUpdateDispatcher(IBufferManager bufferManager, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory)
        {
            _bufferManager = bufferManager;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyUpdate> trades)
        {
            // TODO: update to only dispatch one CurrencyUpdateDTO per type
            IBuffer<CurrencyUpdateDTO> buffer = _bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(trades.Count));
            buffer.Assign(_currencyUpdateDTOFactory.CreateFrom(trades));
            buffer.MarkReady();
        }
    }
}