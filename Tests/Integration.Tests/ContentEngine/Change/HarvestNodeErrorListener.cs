using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine.Change
{
    internal class HarvestNodeErrorListener : ISingleListener<SetHarvestNodeError>
    {
        public Type ListenerType => typeof(SetHarvestNodeError);
        public bool WasCalled { get; private set; }
        public SetHarvestNodeError SetHarvestNodeError { get; private set; }

        public void Handle(SetHarvestNodeError message)
        {
            WasCalled = true;
            SetHarvestNodeError = message;
        }
    }
}