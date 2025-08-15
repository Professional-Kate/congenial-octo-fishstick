using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class InventoryUpdateEntryFactory : IInventoryUpdateEntryFactory
    {
        public InventoryUpdateEntry Create(InventoryUpdate inventoryUpdate, ItemInfo itemInfo, MutateType mutateType)
        {
            return new InventoryUpdateEntry
            {
                InventoryUpdate = inventoryUpdate,
                ItemInfo = itemInfo,
                MutateType = mutateType
            };
        }
    }
}