using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class InventoryUpdateFactory : IInventoryUpdateFactory
    {
        public InventoryUpdate Create(ItemID itemID, uint amount, ActionType actionType)
        {
            return new InventoryUpdate
            {
                ItemID = itemID,
                Amount = amount,
                Action = actionType
            };
        }
    }
}