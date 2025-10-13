using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IInventoryUpdateFactory
    {
        public InventoryUpdate Create(ItemID itemID, uint amount, ActionType actionType);
        
        public InventoryUpdate[] CreateMultiple(ItemID itemID, uint amount, ActionType actionType, uint iterations);
    }
}