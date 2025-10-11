using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationResponseListener : IBufferListener<CurrencyCreationResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationResponse);
        public CurrencyCreationResponse[] CurrencyCreationResponses { get; private set; } = null!;
        public bool WasCalled { get; private set; }
        
        public void Handle(IReadOnlyList<CurrencyCreationResponse> buffer)
        {
            WasCalled = true;
            CurrencyCreationResponses = buffer.ToArray();
        }
    }
}