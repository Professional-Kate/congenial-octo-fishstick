using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.HarvestNode.Loot
{
    internal sealed class InventoryUpdateListener : IBufferListener<InventoryUpdate>
    {
        public Type ListenerType => typeof(InventoryUpdate);
        public bool WasCalled { get; private set; }
        public InventoryUpdate[] InventoryUpdates { get; private set; } = null!;

        public void Handle(IReadOnlyList<InventoryUpdate> buffer)
        {
            WasCalled = true;
            InventoryUpdates = buffer.ToArray();
        }
    }
}