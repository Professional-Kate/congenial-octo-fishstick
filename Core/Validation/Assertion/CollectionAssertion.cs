using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Validation.Assertion
{
    public sealed class CollectionAssertion : ICollectionAssertion
    {
        public void AssertNotNull<T>(IReadOnlyCollection<T>? collection)
        {
            ArgumentNullException.ThrowIfNull(collection);
        }

        public void AssertNotEmpty<T>(IReadOnlyCollection<T> collection)
        {
            if (collection.Count <= 0)
            {
                throw new EmptyCollectionException(typeof(T));
            }
        }

        public void AssertHasElements<T>(IReadOnlyCollection<T>? collection)
        {
            AssertNotNull(collection);
            AssertNotEmpty(collection!);
        }
    }
}