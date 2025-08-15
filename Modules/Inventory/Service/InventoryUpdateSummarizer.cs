using IdelPog.Core.Contracts.Command;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Service
{
    public class InventoryUpdateSummarizer : IInventoryUpdateSummarizer
    {
        public InventoryUpdate[] GetSummary(IReadOnlyList<InventoryUpdate> updates)
        {
            throw new NotImplementedException();
        }
    }
}