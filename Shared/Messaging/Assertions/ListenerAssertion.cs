using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class ListenerAssertion : BaseAssertion, IListenerAssertion
    {
        public ListenerAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertListenerFound(bool wasFound, IListener listener)
        {
            Assert<NoListenerFoundException>(() =>
            {
                if (wasFound == false)
                {
                    throw new NoListenerFoundException(listener);
                }
            });
        }
    }
}