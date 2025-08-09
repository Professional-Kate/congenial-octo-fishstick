using IdelPog.Common.Errors;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
{
    internal class UpdateNodeErrorListener : ISingleListener<HarvestNodeUpdateError>
    {
        public Type ListenerType => typeof(HarvestNodeUpdateError);
        public bool WasCalled { get; private set; }
        public HarvestNodeUpdateError HarvestNodeUpdateError { get; private set; }
        
        public void Handle(HarvestNodeUpdateError message)
        {
            WasCalled = true;
            HarvestNodeUpdateError = message;
        }

    }
}