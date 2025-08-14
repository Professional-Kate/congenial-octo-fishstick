using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation.Assertion
{
    public class CollectionAssertion : BaseAssertion, ICollectionAssertion
    {
        public CollectionAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNotNull<T>(IReadOnlyCollection<T>? collection)
        {
            Assert<ArgumentNullException>(() => { ArgumentNullException.ThrowIfNull(collection); });
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