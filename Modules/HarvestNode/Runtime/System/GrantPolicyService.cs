using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;

namespace IdelPog.HarvestNode.Runtime.System
{
    public sealed class GrantPolicyService<TID> : IGrantPolicyService<TID>
    {
        private readonly IAssetRepository<TID, IGrantPolicy> _grantPolicyRepository;
        private readonly IWeightedPolicyFactory _policyFactory;
        private readonly IUniqueAssertion _uniqueAssertion;

        public GrantPolicyService(IAssetRepository<TID, IGrantPolicy> grantPolicyRepository, IWeightedPolicyFactory policyFactory, IUniqueAssertion uniqueAssertion)
        {
            _grantPolicyRepository = grantPolicyRepository;
            _policyFactory = policyFactory;
            _uniqueAssertion = uniqueAssertion;
        }

        public void CreateGrantPolicy(GrantPolicyEntry grantPolicyEntry, TID id)
        {
            _uniqueAssertion.AssertUnique(id, _grantPolicyRepository.Contains(id));
            
            if (grantPolicyEntry.SkipWeight == 0)
            {
                _grantPolicyRepository.Add(id, new GrantPolicy());
                return;
            }
            
            ILootRoll lootRoll = new DefaultLootRoll();
            WeightedPolicy weightedPolicy = _policyFactory.Create(grantPolicyEntry, lootRoll);
            _grantPolicyRepository.Add(id, weightedPolicy);
        }
    }
}