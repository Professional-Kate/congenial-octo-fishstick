using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Exceptions;

namespace IdelPog.ECS.Assertion
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