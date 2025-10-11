using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode.Loot.Resource
{
    public sealed class ResourceLootCreationResponseListener : IBufferListener<ResourceLootCreationResponse>
    {
        public Type ListenerType => typeof(ResourceLootCreationResponse);
        public bool WasCalled { get; private set; }
        public ResourceLootCreationResponse[] ResourceLootCreationResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<ResourceLootCreationResponse> buffer)
        {
            WasCalled = true;
            ResourceLootCreationResponses = buffer.ToArray();
        }
    }
}