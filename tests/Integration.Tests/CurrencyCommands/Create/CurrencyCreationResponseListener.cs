using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationResponseListener : IBufferListener<CurrencyCreationResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationResponse);
        public IReadOnlyList<CurrencyCreationResponse>? Buffer { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(IReadOnlyList<CurrencyCreationResponse> buffer)
        {
            Buffer = buffer;
            WasCalled = true;
        }
    }
}