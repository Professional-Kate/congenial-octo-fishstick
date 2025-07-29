using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryUpdateResposneFactory : IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse CreateInventoryUpdateDTO(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType)
        {
            return new InventoryUpdateResponse
            {
                ItemInfo = new ItemInfo
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