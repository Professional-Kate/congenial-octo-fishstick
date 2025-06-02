using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Assertions
{
    public class AssertArrayNotEmpty(IHandler handler) : BaseAssertion<ComponentArrayEmptyException>(handler)
    {
        public void Handle(bool arrayHasElements) 
        {
            Assert(() =>
            {
                if (arrayHasElements == false)
                {
                    throw new ComponentArrayEmptyException();
                }
            });
        }
    }
}