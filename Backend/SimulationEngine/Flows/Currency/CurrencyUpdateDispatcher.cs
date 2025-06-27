using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyUpdateDispatcher(IBufferManager bufferManager) : ICurrencyUpdateDispatcher
    {
        public void Dispatch(CurrencyUpdateDTO[] updates)
        {
            IBuffer<CurrencyUpdateDTO> buffer = bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(updates.Length));
            buffer.Assign(updates);
            buffer.MarkReady();
        }
    }
}