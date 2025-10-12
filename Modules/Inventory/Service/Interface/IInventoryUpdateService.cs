using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Service.Interface
{
    public interface IInventoryUpdateService
    {
        public InventoryUpdate CreateRemoveUpdate(ItemID itemID, uint amount);

        public InventoryUpdate CreateAddUpdate(ItemID itemID, uint amount);

        public void ApplyUpdates(IReadOnlyList<InventoryUpdate> inventoryUpdates);
    }
}