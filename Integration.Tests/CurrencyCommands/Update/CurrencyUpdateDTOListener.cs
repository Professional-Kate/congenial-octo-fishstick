using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateDTOListener : IBufferListener<CurrencyUpdateDTO>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateDTO);
        public IReadOnlyList<CurrencyUpdateDTO>? Buffer { get; private set; }
        public bool WasCalled { get; private set; }
        
        public void Handle(IReadOnlyList<CurrencyUpdateDTO> buffer)
        {
            WasCalled = true;
            Buffer = buffer;
        }
    }
}