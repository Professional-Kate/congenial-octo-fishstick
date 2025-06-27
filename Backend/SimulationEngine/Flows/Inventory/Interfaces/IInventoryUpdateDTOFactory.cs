using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public interface IInventoryUpdateDTOFactory
    {
        public InventoryUpdateDTO CreateInventoryUpdateDTO(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType);
    }
}