using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.HarvestNode
{
    public class NodeCreationErrorListener : ISingleListener<HarvestNodeCreationError>
    {
        public Type ListenerType =>  typeof(HarvestNodeCreationError);
        public bool WasCalled { get; private set; }
        public HarvestNodeCreationError HarvestNodeCreationError { get; private set; }

        public void Handle(HarvestNodeCreationError message)
        {
            WasCalled = true;
            HarvestNodeCreationError = message;
        }
    }
}