using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Flows.Currency.Assertions;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyDispatcher(IBufferManager bufferManager, ICurrencyUpdateFactory currencyUpdateFactory, ICurrencyDispatcherAsserter currencyDispatcherAsserter) : ICurrencyDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyTrade> trades)
        {
            currencyDispatcherAsserter.AssertTradeCollection(trades);
            
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs = currencyUpdateFactory.CreateFrom(trades);
            
            IBuffer<CurrencyUpdateDTO> buffer = bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(trades.Count));
            buffer.Assign(updateDTOs.ToArray());
            buffer.MarkReady();
        }
    }
}