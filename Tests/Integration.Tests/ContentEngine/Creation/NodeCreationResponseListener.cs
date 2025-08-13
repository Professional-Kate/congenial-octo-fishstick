using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine.Creation
{
    public class NodeCreationResponseListener : ISingleListener<NodeCreationResponse>
    {
        public Type ListenerType => typeof(NodeCreationResponse);
        public bool WasCalled { get; private set; }
        public NodeCreationResponse NodeCreationResponse { get; private set; }

        public void Handle(NodeCreationResponse message)
        {
            WasCalled = true;
            NodeCreationResponse = message;
        }
    }
}