using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse Create(InventoryUpdateEntry[] inventoryUpdateEntries);
    }
}