using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyCreationErrorDispatcher : ICurrencyCreationErrorDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyCreationErrorFactory _currencyCreationErrorFactory;

        public CurrencyCreationErrorDispatcher(IBufferManager bufferManager, ICurrencyCreationErrorFactory currencyCreationErrorFactory)
        {
            _bufferManager = bufferManager;
            _currencyCreationErrorFactory = currencyCreationErrorFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            IBuffer<CurrencyCreationErrorDTO> buffer = _bufferManager.RequestBuffer<CurrencyCreationErrorDTO>(new BufferRequest(1));
            buffer.Assign([_currencyCreationErrorFactory.CreateCurrencyCreationError(currencyCreations, exception)]);
            buffer.MarkReady();
        }
    }
}