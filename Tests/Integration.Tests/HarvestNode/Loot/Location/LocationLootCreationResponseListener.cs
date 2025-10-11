using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.HarvestNode.Loot.Location
{
    public sealed class LocationLootCreationResponseListener : IBufferListener<LocationLootCreationResponse>
    {
        public Type ListenerType => typeof(LocationLootCreationResponse);
        public bool WasCalled { get; private set; }
        public LocationLootCreationResponse[] LocationLootCreationResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<LocationLootCreationResponse> buffer)
        {
            WasCalled = true;
            LocationLootCreationResponses = buffer.ToArray();
        }
    }
}