using IdelPog.ECS.Exceptions;

namespace IdelPog.ECS.Assertions
{
    public interface IComponentArrayAssertion
    {
        public void AssertNotNull<T>(T[]? array);
        
        public void AssertNotEmpty<T>(T[] array);
        
        /// <summary>
        /// Verifies that <paramref name="array"/> is non-null and contains at least one element
        /// </summary>
        /// <exception cref="ComponentArrayNullException"/>
        /// <exception cref="ComponentArrayEmptyException"/>
        public void AssertHasElements<T>(T[]? array);
    }
}