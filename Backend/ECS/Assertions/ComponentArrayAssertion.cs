using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Assertions
{
    public class ComponentArrayAssertion : BaseAssertion, IComponentArrayAssertion
    {
        public ComponentArrayAssertion(IHandler handler) : base(handler)
        {
        }
        
        public void AssertNotNull<T>(T[]? array)
        {
            Assert<ComponentArrayNullException>(() =>
            {
                if (array == null)
                {
                    throw new ComponentArrayNullException();
                }
            });
        }
        
        public void AssertNotEmpty<T>(T[] array)
        {
            Assert<ComponentArrayEmptyException>(() =>
            {
                if (array.Length == 0)
                {
                    throw new ComponentArrayEmptyException();
                }
            });
        }

        public void AssertHasElements<T>(T[]? array)
        {
            AssertNotNull(array);
            AssertNotEmpty(array!);
        }
    }
}