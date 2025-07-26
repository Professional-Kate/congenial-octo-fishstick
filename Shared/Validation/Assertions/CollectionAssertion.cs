using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class CollectionAssertion : BaseAssertion, ICollectionAssertion
    {
        public CollectionAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNotNull<T>(IReadOnlyCollection<T>? collection)
        {
            Assert<ArgumentNullException>(() =>
            { 
                ArgumentNullException.ThrowIfNull(collection);
            });
        }

        public void AssertNotEmpty<T>(IReadOnlyCollection<T> collection)
        {
            Assert<EmptyCollectionException>(() =>
            {
                if (collection.Count <= 0)
                {
                    throw new EmptyCollectionException(typeof(T));
                }
            });
        }

        public void AssertHasElements<T>(IReadOnlyCollection<T>? collection)
        {
            AssertNotNull(collection);
            AssertNotEmpty(collection!);
        }
    }
}