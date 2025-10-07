using IdelPog.Core.Contracts.Command;

namespace IdelPog.HarvestNode.Runtime.System.Interface
{
    public interface IHarvestNodeLootService
    {
        public IReadOnlyList<InventoryUpdate> GenerateInventoryUpdates(Contracts.HarvestNode harvestNode);
    }
}