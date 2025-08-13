using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation.Assertion
{
    public class UniqueAssertion : BaseAssertion, IUniqueAssertion
    {
        public UniqueAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertUnique<TKey>(TKey key, bool exists)
        {
            Assert<DuplicateEntityException>(() =>
            {
                if (exists)
                {
                    throw new DuplicateEntityException(key!);
                }
            });
        }
    }
}