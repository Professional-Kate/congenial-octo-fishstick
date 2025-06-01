using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Assertions
{
    public class AssertComponentDoesNotExist(IHandler handler) : BaseAssertion<ComponentAlreadyExistsException>(handler)
    {
        public void Handle(bool componentAlreadyExists, IComponent componentContext)
        {
            Assert(() =>
            {
                if (componentAlreadyExists)
                {
                    throw new ComponentAlreadyExistsException(componentContext);
                }
            });
        }
    }
}