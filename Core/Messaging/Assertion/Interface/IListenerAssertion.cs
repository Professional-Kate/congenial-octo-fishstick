using IdelPog.Core.Messaging.Listener;

namespace IdelPog.Core.Messaging.Assertion.Interface
{
    public interface IListenerAssertion
    {
        public void AssertListenerFound(bool wasFound, IListener listener);
    }
}