using IdelPog.Common.Errors;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateErrorListener : ISingleListener<CurrencyUpdateError>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateError);
        public CurrencyUpdateError CurrencyUpdateError { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyUpdateError message)
        {
            WasCalled = true;
            CurrencyUpdateError = message;
        }
    }
}