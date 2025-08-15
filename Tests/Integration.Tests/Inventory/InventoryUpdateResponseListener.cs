using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.Inventory
{
    public class InventoryUpdateResponseListener : ISingleListener<InventoryUpdateResponse>
    {
        public Type ListenerType => typeof(InventoryUpdateResponse);
        public bool WasCalled { get; private set; }
        public InventoryUpdateResponse InventoryUpdateResponse { get; private set; }

        public void Handle(InventoryUpdateResponse message)
        {
            WasCalled = true;
            InventoryUpdateResponse = message;
        }
    }
}