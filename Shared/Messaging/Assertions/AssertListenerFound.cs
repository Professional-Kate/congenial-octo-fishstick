using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messaging;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
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