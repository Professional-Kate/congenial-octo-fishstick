using IdelPog.Common.Errors;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
{
    public class HarvestNodeErrorListener : ISingleListener<SetHarvestNodeError>
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