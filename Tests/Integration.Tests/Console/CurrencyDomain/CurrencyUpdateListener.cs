using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.Console.CurrencyDomain
{
    internal class CurrencyUpdateListener : IBufferListener<CurrencyUpdate>
    {
        public Type ListenerType => typeof(CurrencyUpdate);
        public bool WasCalled { get; private set; }
        public IReadOnlyList<CurrencyUpdate>? Buffer { get; private set; }

        public void Handle(IReadOnlyList<CurrencyUpdate> buffer)
        {
            WasCalled = true;
            Buffer = buffer;
        }
    }
}