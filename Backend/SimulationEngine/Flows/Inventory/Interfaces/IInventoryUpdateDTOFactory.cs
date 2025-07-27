using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public interface IInventoryUpdateDTOFactory
    {
        public InventoryUpdateDTO CreateInventoryUpdateDTO(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType);
    }
}