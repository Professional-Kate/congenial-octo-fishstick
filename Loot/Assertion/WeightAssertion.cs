using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Exceptions;

namespace IdelPog.Loot.Assertion
{
    public class WeightAssertion : BaseAssertion, IWeightAssertion
    {
        public WeightAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertWeightIsNotZero(uint weight)
        {
            Assert<ZeroWeightException>(() =>
            {
                if (weight <= 0)
                {
                    throw new ZeroWeightException();
                }
            });
        }
    }
}