using IdelPog.Messaging.Listeners;

namespace IdelPog.Messaging.Assertions
{
    public interface IListenerAssertion
    {
        public void AssertListenerFound(bool wasFound, IListener listener);
    }
}