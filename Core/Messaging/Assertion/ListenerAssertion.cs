using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
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