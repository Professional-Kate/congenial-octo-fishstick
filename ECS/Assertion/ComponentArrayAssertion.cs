using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Exceptions;

namespace IdelPog.ECS.Assertion
{
    public sealed class ComponentArrayAssertion : IComponentArrayAssertion
    {
        public void AssertNotNull<T>(T[]? array)
        {
            if (array == null)
            {
                throw new ComponentArrayNullException();
            }
        }

        public void AssertNotEmpty<T>(T[] array)
        {
            if (array.Length == 0)
            {
                throw new ComponentArrayEmptyException();
            }
        }

        public void AssertHasElements<T>(T[]? array)
        {
            AssertNotNull(array);
            AssertNotEmpty(array!);
        }
    }
}