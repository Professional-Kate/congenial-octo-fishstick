using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryUpdateDTOFactory : IInventoryUpdateDTOFactory
    {
        public InventoryUpdateDTO CreateInventoryUpdateDTO(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType)
        {
            return new InventoryUpdateDTO
            {
                ItemDTO = new ItemDTO
                {
                    Amount = item.Amount,
                    ItemID = item.ItemID,
                    BaseSellPrice = item.BaseSellPrice
                },
                ActionType = inventoryUpdate.Action,
                MutateType = mutateType
            };
        }
    }
}