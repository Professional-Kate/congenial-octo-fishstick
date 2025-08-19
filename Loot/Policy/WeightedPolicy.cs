using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Random;

namespace IdelPog.Loot.Policy
{
    public sealed class WeightedPolicy : IGrantPolicy
    {
        private readonly ILootRoll _lootRoll;
        private readonly int _grantWeight;
        private readonly int _skipWeight;

        public WeightedPolicy(ILootRoll lootRoll, int grantWeight, int skipWeight, IWeightAssertion weightAssertion)
        {
            weightAssertion.AssertWeightIsPositive(grantWeight);
            weightAssertion.AssertWeightIsPositive(skipWeight);
            
            _lootRoll = lootRoll;
            _grantWeight = grantWeight;
            _skipWeight = skipWeight;
        }

        public bool ShouldGrant()
        {
            int totalWeight = _grantWeight + _skipWeight;
            int roll = _lootRoll.ExclusiveNextInt(0,  totalWeight);
            return roll < _grantWeight;
        }
    }
}