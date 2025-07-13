using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class AssertCollectionNotEmpty(IHandler handler) : BaseAssertion<CollectionEmptyException>(handler), IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection)
        {
            Assert(() =>
            {
                if (collection.Count == 0)
                {
                    throw new CollectionEmptyException(typeof(T));
                }
            });
        } 
    }
}