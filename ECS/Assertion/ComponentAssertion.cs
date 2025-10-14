using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Exceptions;

namespace IdelPog.ECS.Assertion
{
    public sealed class ComponentAssertion : IComponentAssertion
    {

        public void AssertUnique<TComponent>(bool exists)
        {
            if (exists)
            {
                throw new ComponentAlreadyExistsException(typeof(TComponent));
            }
        }

        public void AssertFound<TComponent>(bool found)
        {
            if (found == false)
            {
                throw new ComponentNotFoundException(typeof(TComponent));
            }
        }
    }
}