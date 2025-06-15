using IdelPog.Staging.Exceptions;
using IdelPog.Staging.Messaging;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Staging.Assertions
{
    public class AssertListenerFound(IHandler handler) : BaseAssertion<NoListenerFoundException>(handler), IAssertListenerFound
    {
        public void AssertFound(IListener listener, bool wasFound)
        {
            Assert(() =>
            {
                if (wasFound == false)
                {
                    throw new NoListenerFoundException(listener);
                }
            });
        }
    }
}