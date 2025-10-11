using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Inventory.Contracts.Response;

namespace IdelPog.Integration.Tests.Inventory
{
    public class InventoryUpdateResponseListener : IBufferListener<InventoryUpdateResponse>
    {
        public Type ListenerType => typeof(InventoryUpdateResponse);
        public bool WasCalled { get; private set; }
        public InventoryUpdateResponse[] InventoryUpdateResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<InventoryUpdateResponse> buffer)
        {
            WasCalled = true;
            InventoryUpdateResponses = buffer.ToArray();
        }
    }
}