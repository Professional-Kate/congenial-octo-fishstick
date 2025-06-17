using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyDispatcher(IBufferManager bufferManager) : ICurrencyDispatcher
    {
        public void Dispatch(CurrencyTrade trade)
        {
            IBuffer<CurrencyUpdateDTO> buffer = bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1));

            CurrencyUpdateDTO currencyUpdateDTO = new()
            {
                Amount = trade.Amount,
                Currency = trade.Currency,
                Action = trade.Action
            };
            
            buffer.Assign([currencyUpdateDTO]);
            buffer.MarkReady();
        }
    }
}