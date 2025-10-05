using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.CurrencyCommands.Update
{
    internal sealed class CurrencyUpdateResponseListener : IBufferListener<CurrencyUpdateResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateResponse);
        public CurrencyUpdateResponse[] CurrencyUpdateResponses { get; private set; } = null!;
        public bool WasCalled { get; private set; }

        public void Handle(IReadOnlyList<CurrencyUpdateResponse> buffer)
        {
            WasCalled = true;
            CurrencyUpdateResponses = buffer.ToArray();
        }
    }
}