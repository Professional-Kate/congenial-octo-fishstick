using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class InventoryUpdateResponseFactory : IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse Create(InventoryUpdateEntry[] inventoryUpdateEntries)
        {
            return new InventoryUpdateResponse
            {
                InventoryUpdateEntry = inventoryUpdateEntries
            };
        }
    }
}