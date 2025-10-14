using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Validation.Assertion
{
    public sealed class FoundAssertion : IFoundAssertion
    {
        public void AssertFound<TKey>(TKey key, bool found)
        {
            if (found == false)
            {
                throw new NotFoundException<TKey>(key);
            }
        }
    }
}