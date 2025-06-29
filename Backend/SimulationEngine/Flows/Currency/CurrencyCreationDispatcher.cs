using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationDispatcher : ICurrencyCreationDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyCreationFactory _currencyCreationFactory;
        
        public CurrencyCreationDispatcher(IBufferManager bufferManager, ICurrencyCreationFactory currencyCreationFactory)
        {
            _bufferManager = bufferManager;
            _currencyCreationFactory = currencyCreationFactory;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyCreation> createdCurrency)
        {
            IBuffer<CurrencyCreationDTO> buffer = _bufferManager.RequestBuffer<CurrencyCreationDTO>(new BufferRequest(createdCurrency.Count));
            buffer.Assign(_currencyCreationFactory.CreateFrom(createdCurrency));
            buffer.MarkReady();
        }
    }
}