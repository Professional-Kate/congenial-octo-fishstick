using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public interface IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse CreateInventoryUpdateDTO(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType);
    }
}