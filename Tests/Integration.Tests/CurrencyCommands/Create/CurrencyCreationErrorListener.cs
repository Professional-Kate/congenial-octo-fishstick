using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Currency.Contracts.Error;

namespace IdelPog.Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationErrorListener : ISingleListener<CurrencyCreationError>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationError);
        public CurrencyCreationError CurrencyUpdateError { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyCreationError message)
        {
            CurrencyUpdateError = message;
            WasCalled = true;
        }
    }
}