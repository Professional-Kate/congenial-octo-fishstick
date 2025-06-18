using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyDispatcher(IBufferManager bufferManager, ICurrencyUpdateFactory currencyUpdateFactory) : ICurrencyDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyTrade> trades)
        {
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs = currencyUpdateFactory.CreateFrom(trades);
            
            IBuffer<CurrencyUpdateDTO> buffer = bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1));
            buffer.Assign(updateDTOs.ToArray());
            buffer.MarkReady();
        }
    }
}