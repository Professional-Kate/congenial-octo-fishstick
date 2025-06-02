using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Assertions
{
    public class AssertComponentFound(IHandler handler) : BaseAssertion<ComponentNotFoundException>(handler)
    {
        public void Handle(bool componentWasFound, Type componentContext)
        {
            Assert(() =>
            {
                if (componentWasFound == false)
                {
                    throw new ComponentNotFoundException(componentContext);
                }
            });
        }
    }
}