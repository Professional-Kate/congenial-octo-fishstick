using IdelPog.Core.Contracts.Command;

namespace IdelPog.Inventory.Service.Interface
{
    public interface IInventoryUpdateSummarizer
    {
        public InventoryUpdate[] GetSummary(IReadOnlyList<InventoryUpdate> updates);
    }
}