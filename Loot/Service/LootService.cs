using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Loot.Contracts.Grant;
using IdelPog.Loot.Contracts.Table;
using IdelPog.Loot.Service.Interface;

namespace IdelPog.Loot.Service
{
    public class LootService : ILootService
    {
        private readonly IAssetRepository<ItemID, ILootTable> _lootTableRepository;
        private readonly IDispatchOne<InventoryUpdate> _inventoryUpdateDispatcher;
        private readonly IGrantPolicy _grantPolicy;
        private readonly IFoundAssertion _foundAssertion;

        public LootService(IAssetRepository<ItemID, ILootTable> lootTableRepository, IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher, IGrantPolicy grantPolicy, IFoundAssertion foundAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _grantPolicy = grantPolicy;
            _foundAssertion = foundAssertion;
        }

        public void DispatchInventoryUpdates(ItemID itemID)
        {
            if (_grantPolicy.ShouldGrant() == false)
            {
                return;
            }
            
            _foundAssertion.AssertFound(itemID, _lootTableRepository.Contains(itemID));
            
            ILootTable lootTable = _lootTableRepository.Get(itemID);
            ItemID lootID = lootTable.Roll();
            
            _inventoryUpdateDispatcher.Dispatch(new InventoryUpdate { ItemID = lootID, ActionType = ActionType.ADD, Amount = 1});
        }
    }
}