using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Validation.Assertion
{
    public sealed class UniqueAssertion : IUniqueAssertion
    {
        public void AssertUnique<TKey>(TKey key, bool exists)
        {
            if (exists)
            {
                throw new DuplicateEntityException(key!);
            }
        }
    }
}