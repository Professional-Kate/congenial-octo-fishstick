using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public class InventoryUpdateDispatcher(IBufferManager bufferManager) : IInventoryUpdateDispatcher
    {
        public void DispatchUpdates(InventoryUpdateDTO[] inventoryUpdates)
        {
            IBuffer<InventoryUpdateDTO> buffer = bufferManager.RequestBuffer<InventoryUpdateDTO>(new BufferRequest(inventoryUpdates.Length));
            buffer.Assign(inventoryUpdates);
            buffer.MarkReady();
        }
    }
}