using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation.Assertion
{
    public class FoundAssertion : BaseAssertion, IFoundAssertion
    {
        public FoundAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertFound<TKey>(TKey key, bool found)
        {
            Assert<NotFoundException<TKey>>(() =>
            {
                if (found == false)
                {
                    throw new NotFoundException<TKey>(key);
                }
            });
        }
    }
}