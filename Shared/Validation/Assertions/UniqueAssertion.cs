using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
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