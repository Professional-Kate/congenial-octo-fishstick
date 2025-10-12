using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Service
{
    public sealed class InventoryUpdateService : IInventoryUpdateService
    {
        public InventoryUpdate CreateRemoveUpdate(ItemID itemID, uint amount)
        {
            throw new NotImplementedException();
        }

        public InventoryUpdate CreateAddUpdate(ItemID itemID, uint amount)
        {
            throw new NotImplementedException();
        }

        public void ApplyUpdates(IReadOnlyList<InventoryUpdate> inventoryUpdates)
        {
            throw new NotImplementedException();
        }
    }
}