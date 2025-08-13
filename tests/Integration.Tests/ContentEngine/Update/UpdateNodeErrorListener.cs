using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine.Update
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