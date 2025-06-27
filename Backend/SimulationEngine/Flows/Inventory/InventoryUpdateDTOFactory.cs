using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Flows.Inventory
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
                    ItemID = item.ID,
                    SellPrice = item.SellPrice
                },
                ActionType = inventoryUpdate.Action,
                MutateType = mutateType
            };
        }
    }
}