using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IInventoryUpdateEntryFactory
    {
        public InventoryUpdateEntry Create(InventoryUpdate inventoryUpdate, ItemInfo itemInfo, MutateType mutateType);
    }
}