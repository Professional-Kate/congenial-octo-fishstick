using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyDispatcher(IBufferManager bufferManager, ICurrencyUpdateFactory currencyUpdateFactory) : ICurrencyDispatcher
    {
        public void Dispatch(CurrencyTrade trade)
        {
            CurrencyUpdateDTO updateDTO = currencyUpdateFactory.CreateFrom(trade);
            
            IBuffer<CurrencyUpdateDTO> buffer = bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1));
            buffer.Assign([updateDTO]);
            buffer.MarkReady();
        }
    }
}