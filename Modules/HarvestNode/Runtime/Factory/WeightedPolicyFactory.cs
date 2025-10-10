using IdelPog.Core.Contracts;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public sealed class WeightedPolicyFactory : IWeightedPolicyFactory
    {
        private readonly IWeightAssertion _weightAssertion;

        public WeightedPolicyFactory(IWeightAssertion weightAssertion)
        {
            _weightAssertion = weightAssertion;
        }

        public WeightedPolicy Create(GrantPolicyEntry grantPolicyEntry, ILootRoll lootRoll)
        {
            WeightedPolicy weightedPolicy = new(lootRoll, grantPolicyEntry.GrantWeight, grantPolicyEntry.SkipWeight, _weightAssertion);
            return  weightedPolicy;
        }
    }
}