using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateResponseListener : IBufferListener<CurrencyUpdateResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateResponse);
        public IReadOnlyList<CurrencyUpdateResponse>? Buffer { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(IReadOnlyList<CurrencyUpdateResponse> buffer)
        {
            WasCalled = true;
            Buffer = buffer;
        }
    }
}