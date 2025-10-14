using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Exceptions;

namespace IdelPog.Loot.Assertion
{
    public sealed class WeightAssertion : IWeightAssertion
    {
        public void AssertWeightIsPositive(int weight)
        {
            if (weight < 0)
            {
                throw new InvalidWeightException();
            }
        }
    }
}