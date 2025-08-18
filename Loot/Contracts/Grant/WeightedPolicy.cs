using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Random;

namespace IdelPog.Loot.Contracts.Grant
{
    public sealed class WeightedPolicy : IGrantPolicy
    {
        private readonly ILootRoll _lootRoll;
        private readonly uint _grantWeight;
        private readonly uint _skipWeight;

        public WeightedPolicy(ILootRoll lootRoll, uint grantWeight, uint skipWeight, IWeightAssertion weightAssertion)
        {
            weightAssertion.AssertWeightIsNotZero(grantWeight);
            weightAssertion.AssertWeightIsNotZero(skipWeight);
            
            _lootRoll = lootRoll;
            _grantWeight = grantWeight;
            _skipWeight = skipWeight;
        }

        public bool ShouldGrant()
        {
            uint totalWeight = _grantWeight + _skipWeight;
            uint roll = _lootRoll.ExclusiveNextInt(0,  totalWeight);
            return roll < _grantWeight;
        }
    }
}