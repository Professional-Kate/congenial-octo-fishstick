using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener;

namespace IdelPog.Core.Messaging.Assertion
{
    public sealed class ListenerAssertion : IListenerAssertion
    {
        public void AssertListenerFound(bool wasFound, IListener listener)
        {
            if (wasFound == false)
            {
                throw new NoListenerFoundException(listener);
            }
        }
    }
}