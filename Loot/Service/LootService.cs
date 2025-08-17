using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Loot.Contracts;
using IdelPog.Loot.Service.Interface;

namespace IdelPog.Loot.Service
{
    public class LootService : ILootService
    {
        private readonly IAssetRepository<ItemID, WeightedLootTable> _lootTableRepository;
        private readonly IDispatchMany<InventoryUpdate> _inventoryUpdateDispatcher;
        private readonly IFoundAssertion _foundAssertion;

        public LootService(IAssetRepository<ItemID, WeightedLootTable> lootTableRepository, IDispatchMany<InventoryUpdate> inventoryUpdateDispatcher, IFoundAssertion foundAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _foundAssertion = foundAssertion;
        }

        public void GenerateInventoryUpdates(ItemID itemID)
        {
            _foundAssertion.AssertFound(itemID, _lootTableRepository.Contains(itemID));
            
            WeightedLootTable lootTable = _lootTableRepository.Get(itemID);
            ItemID lootID = lootTable.Roll();
            
            _inventoryUpdateDispatcher.Dispatch([new InventoryUpdate { ItemID = lootID, ActionType = ActionType.ADD, Amount = 1}]);
        }
    }
}