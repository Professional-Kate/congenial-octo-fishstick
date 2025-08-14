using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class InventoryUpdateResponseFactory : IInventoryUpdateResponseFactory
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