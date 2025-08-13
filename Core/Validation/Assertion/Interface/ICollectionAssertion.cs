using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Validation.Assertion.Interface
{
    public interface ICollectionAssertion
    {
        public void AssertNotNull<T>(IReadOnlyCollection<T>? collection);

        public void AssertNotEmpty<T>(IReadOnlyCollection<T> collection);

        /// <summary>
        /// Verifies that <paramref name="collection"/> is non-null and contains at least one element
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EmptyCollectionException"/>
        public void AssertHasElements<T>(IReadOnlyCollection<T>? collection);
    }
}