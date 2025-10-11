using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.HarvestNode.Loot.Resource
{
    public sealed class ResourceLootCreationErrorListener : ISingleListener<ResourceLootCreationError>
    {
        public Type ListenerType => typeof(ResourceLootCreationError);
        public bool WasCalled { get; private set; }
        public ResourceLootCreationError ResourceLootCreationError { get; private set; }
        
        public void Handle(ResourceLootCreationError message)
        {
            WasCalled = true;
            ResourceLootCreationError = message;
        }

    }
}