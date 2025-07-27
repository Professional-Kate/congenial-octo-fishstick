using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationDTOListener : IBufferListener<CurrencyCreationDTO>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationDTO);
        public IReadOnlyList<CurrencyCreationDTO>? Buffer { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(IReadOnlyList<CurrencyCreationDTO> buffer)
        {
            Buffer = buffer;
            WasCalled = true;
        }
    }
}