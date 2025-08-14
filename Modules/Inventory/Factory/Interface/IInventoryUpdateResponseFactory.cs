using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Inventory.Contracts;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse Create(Item item, InventoryUpdate inventoryUpdate, MutateType mutateType);
    }
}