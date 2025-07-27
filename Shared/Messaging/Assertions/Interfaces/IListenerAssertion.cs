using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Assertions
{
    public interface IListenerAssertion
    {
        public void AssertListenerFound(bool wasFound, IListener listener);
    }
}