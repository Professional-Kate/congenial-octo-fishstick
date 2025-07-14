using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyUpdateDispatcher : ICurrencyUpdateDispatcher
    {
        private readonly IBufferManager _bufferManager;
        private readonly ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory;
        private readonly ICurrencyUpdateDispatcherAsserter _currencyUpdateDispatcherAsserter;
        
        public CurrencyUpdateDispatcher(IBufferManager bufferManager, ICurrencyUpdateDTOFactory currencyUpdateDTOFactory,  ICurrencyUpdateDispatcherAsserter currencyUpdateDispatcherAsserter)
        {
            _bufferManager = bufferManager;
            _currencyUpdateDTOFactory = currencyUpdateDTOFactory;
            _currencyUpdateDispatcherAsserter = currencyUpdateDispatcherAsserter;
        }
        
        public void Dispatch(IReadOnlyList<CurrencyUpdate> trades)
        {
            _currencyUpdateDispatcherAsserter.AssertTradeCollection(trades);
            IBuffer<CurrencyUpdateDTO> buffer = _bufferManager.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(trades.Count));
            buffer.Assign(_currencyUpdateDTOFactory.CreateFrom(trades));
            buffer.MarkReady();
        }
    }
}