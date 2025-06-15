using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Assertions
{
    public class AssertComponentDoesNotExist(IHandler handler) : BaseAssertion<ComponentAlreadyExistsException>(handler)
    {
        public void Handle(bool componentAlreadyExists, object componentContext)
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