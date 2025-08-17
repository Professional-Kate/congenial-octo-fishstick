using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine
{
    public class NodeCreationErrorListener : ISingleListener<NodeCreationError>
    {
        public Type ListenerType =>  typeof(NodeCreationError);
        public bool WasCalled { get; private set; }
        public NodeCreationError NodeCreationError { get; private set; }

        public void Handle(NodeCreationError message)
        {
            WasCalled = true;
            NodeCreationError = message;
        }
    }
}