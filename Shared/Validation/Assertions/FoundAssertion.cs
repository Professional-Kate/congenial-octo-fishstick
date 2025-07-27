using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
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