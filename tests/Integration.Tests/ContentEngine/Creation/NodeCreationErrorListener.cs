using IdelPog.Common.Errors;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
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