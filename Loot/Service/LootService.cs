using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Service.Interface;
using IdelPog.Loot.Table;

namespace IdelPog.Loot.Service
{
    public sealed class LootService<TID> : ILootService<TID> where TID : Enum
    {
        private readonly IAssetRepository<TID, ILootTable> _lootTableRepository;
        private readonly IDispatchOne<InventoryUpdate> _inventoryUpdateDispatcher;
        private readonly IGrantPolicy _grantPolicy;
        private readonly IFoundAssertion _foundAssertion;

        public LootService(IAssetRepository<TID, ILootTable> lootTableRepository, IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher, IGrantPolicy grantPolicy, IFoundAssertion foundAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _grantPolicy = grantPolicy;
            _foundAssertion = foundAssertion;
        }

        public void DispatchInventoryUpdates(TID id)
        {
            if (_grantPolicy.ShouldGrant() == false)
            {
                return;
            }
            
            _foundAssertion.AssertFound(id, _lootTableRepository.Contains(id));
            
            ILootTable lootTable = _lootTableRepository.Get(id);
            ItemID lootID = lootTable.Roll();
            
            _inventoryUpdateDispatcher.Dispatch(new InventoryUpdate { ItemID = lootID, ActionType = ActionType.ADD, Amount = 1});
        }
    }
}