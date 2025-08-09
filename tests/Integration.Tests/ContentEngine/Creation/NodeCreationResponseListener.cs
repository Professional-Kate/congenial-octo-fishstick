using IdelPog.Common.Responses;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
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