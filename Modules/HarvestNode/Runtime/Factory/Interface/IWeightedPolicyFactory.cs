using IdelPog.HarvestNode.Contracts;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface IWeightedPolicyFactory
    {
        public WeightedPolicy Create(GrantPolicyEntry grantPolicyEntry, ILootRoll lootRoll);
    }
}