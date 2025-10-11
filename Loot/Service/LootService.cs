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
        private readonly IAssetRepository<TID, IGrantPolicy> _grantPolicyRepository;
        private readonly IFoundAssertion _foundAssertion;

        public LootService(IAssetRepository<TID, ILootTable> lootTableRepository, IAssetRepository<TID, IGrantPolicy> grantPolicyRepository, IFoundAssertion foundAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _grantPolicyRepository = grantPolicyRepository;
            _foundAssertion = foundAssertion;
        }

        public bool ShouldGrant(TID id)
        {
            _foundAssertion.AssertFound(id, _grantPolicyRepository.Contains(id));
            
            IGrantPolicy policy = _grantPolicyRepository.Get(id);
            bool shouldGrant = policy.ShouldGrant();
            
            return shouldGrant;
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