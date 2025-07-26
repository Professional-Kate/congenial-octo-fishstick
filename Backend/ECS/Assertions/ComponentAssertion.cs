using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Assertions
{
    public class ComponentAssertion : BaseAssertion, IComponentAssertion
    {
        public ComponentAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertUnique<TComponent>(bool exists)
        {
            Assert<ComponentAlreadyExistsException>(() =>
            {
                if (exists)
                {
                    throw new ComponentAlreadyExistsException(typeof(TComponent));
                }
            });
        }

        public void AssertFound<TComponent>(bool found)
        {
            Assert<ComponentNotFoundException>(() =>
            {
                if (found == false)
                {
                    throw new ComponentNotFoundException(typeof(TComponent));
                }
            });
        }
    }
}