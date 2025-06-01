using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Assertions
{
    public class AssertArrayNotNull(IHandler handler) : BaseAssertion<ComponentArrayNullException>(handler)
    {
        public void Handle(bool arrayNotNull)
        {
            Assert(() =>
            {
                if (arrayNotNull == false)
                {
                    throw new ComponentArrayNullException();
                }
            });
        }
    }
}