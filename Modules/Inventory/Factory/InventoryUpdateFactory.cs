using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public sealed class InventoryUpdateFactory : IInventoryUpdateFactory
    {
        public InventoryUpdate Create(ItemID itemID, uint amount, ActionType actionType)
        {
            return new InventoryUpdate
            {
                ItemID = itemID,
                Amount = amount,
                ActionType = actionType
            };
        }

        public InventoryUpdate[] CreateMultiple(ItemID itemID, uint amount, ActionType actionType, uint iterations)
        {
            InventoryUpdate[] updates = new InventoryUpdate[iterations];
            for (int i = 0; i < iterations; i++)
            {
                updates[i] = Create(itemID, amount, actionType);
            }

            return updates;
        }
    }
}