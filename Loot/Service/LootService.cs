using IdelPog.Core.Contracts.Enum;
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
        private readonly IGrantPolicy _grantPolicy;
        private readonly IFoundAssertion _foundAssertion;

        public LootService(IAssetRepository<TID, ILootTable> lootTableRepository, IGrantPolicy grantPolicy, IFoundAssertion foundAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _grantPolicy = grantPolicy;
            _foundAssertion = foundAssertion;
        }

        public bool ShouldGrant()
        {
            return _grantPolicy.ShouldGrant();
        }

        public ItemID GenerateItemID(TID id)
        {
            _foundAssertion.AssertFound(id, _lootTableRepository.Contains(id));
            
            ILootTable lootTable = _lootTableRepository.Get(id);
            ItemID lootID = lootTable.Roll();

            return lootID;
        }
    }
}