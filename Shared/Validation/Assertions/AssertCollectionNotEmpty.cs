using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class AssertCollectionNotEmpty(IHandler handler) : BaseAssertion<EmptyCollectionException>(handler), IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection)
        {
            RunAssertion(collection);
        }

        public void Handle<T>(ReadOnlySpan<T> collection)
        {
            T[] collectionArray = collection.ToArray();
            RunAssertion(collectionArray);
        }

        private void RunAssertion<T>(IReadOnlyList<T> collection)
        {
            Assert(() =>
            {
                if (collection.Count == 0)
                {
                    throw new EmptyCollectionException(collection);
                }
            });
        }
    }
}