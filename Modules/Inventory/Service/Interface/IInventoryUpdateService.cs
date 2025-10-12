using IdelPog.Core.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;

namespace IdelPog.Inventory.Service.Interface
{
    public interface IInventoryUpdateService
    {
        public IReadOnlyList<InventoryUpdateResponse> ApplyUpdates(IReadOnlyList<InventoryUpdate> inventoryUpdates);
    }
}