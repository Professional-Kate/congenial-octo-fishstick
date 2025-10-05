using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine
{
    public class NodeCreationResponseListener : ISingleListener<HarvestNodeCreationResponse>
    {
        public Type ListenerType => typeof(HarvestNodeCreationResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeCreationResponse HarvestNodeCreationResponse { get; private set; }

        public void Handle(HarvestNodeCreationResponse message)
        {
            WasCalled = true;
            HarvestNodeCreationResponse = message;
        }
    }
}