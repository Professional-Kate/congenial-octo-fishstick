using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Assertions
{
    public class AssertArrayNotNull(IHandler handler) : BaseAssertion<ComponentArrayNullException>(handler), IAssertArrayNotNull
    {
        public void Handle<T>(T[]? arrayNotNull)
        {
            Assert(() =>
            {
                if (arrayNotNull == null)
                {
                    throw new ComponentArrayNullException();
                }
            });
        }
    }
}