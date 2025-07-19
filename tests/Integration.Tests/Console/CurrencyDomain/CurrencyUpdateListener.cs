using IdelPog.Common.Enums;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.Console
{
    internal class CurrencyUpdateListener : IBufferListener<CurrencyUpdate>
    {
        public Type ListenerType => typeof(CurrencyUpdate);
        public bool WasCalled { get; private set; }
        public IReadOnlyList<CurrencyUpdate> Buffer { get; private set; }

        public void Handle(IReadOnlyList<CurrencyUpdate> buffer)
        {
            WasCalled = true;
            Buffer = buffer;
        }
    }
}