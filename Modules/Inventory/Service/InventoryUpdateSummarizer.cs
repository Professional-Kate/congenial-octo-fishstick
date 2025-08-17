using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Service
{
    public class InventoryUpdateSummarizer : IInventoryUpdateSummarizer
    {
        private readonly IInventoryUpdateFactory _inventoryUpdateFactory;
        private readonly ICollectionAssertion _collectionAssertions;

        public InventoryUpdateSummarizer(IInventoryUpdateFactory inventoryUpdateFactory, ICollectionAssertion collectionAssertions)
        {
            _inventoryUpdateFactory = inventoryUpdateFactory;
            _collectionAssertions = collectionAssertions;
        }

        public InventoryUpdate[] GetSummary(IReadOnlyList<InventoryUpdate> updates)
        {
            _collectionAssertions.AssertHasElements(updates);

            Dictionary<ItemID, RunningUpdate> amounts = SummarizeAmounts(updates);
            InventoryUpdate[] summaryUpdates = CreateSummaryUpdates(amounts);
            
            return summaryUpdates;
        }

        private Dictionary<ItemID, RunningUpdate> SummarizeAmounts(IReadOnlyList<InventoryUpdate> updates)
        {
            Dictionary<ItemID, RunningUpdate> amounts = new();

            foreach (InventoryUpdate inventoryUpdate in updates)
            {
                amounts.TryAdd(inventoryUpdate.ItemID, new RunningUpdate());
                amounts[inventoryUpdate.ItemID].Apply(inventoryUpdate.ActionType, inventoryUpdate.Amount);
            }
            
            return amounts;
        }

        private InventoryUpdate[] CreateSummaryUpdates(Dictionary<ItemID, RunningUpdate> amounts)
        {
            List<InventoryUpdate> updates = [];

            foreach ((ItemID itemID, RunningUpdate runningAmount) in amounts)
            {
                if (runningAmount.IsZeroAmount())
                {
                    continue;
                }
                
                ActionType action;
                uint inventoryUpdateAmount = runningAmount.AddAmount;

                if (runningAmount.RemoveAmount > inventoryUpdateAmount)
                {
                    action = ActionType.REMOVE;
                    inventoryUpdateAmount = runningAmount.RemoveAmount - runningAmount.AddAmount;
                }
                else
                {
                    action = ActionType.ADD;
                    inventoryUpdateAmount -=  runningAmount.RemoveAmount;
                }
                
                updates.Add(_inventoryUpdateFactory.Create(itemID, inventoryUpdateAmount, action));
            }
            
            return updates.ToArray();
        }
    }
}