using IdelPog.Messaging.Listeners;

namespace IdelPog.Messaging.Assertions
{
    public interface IAssertListenerFound
    {
        public void AssertFound(IListener listener, bool wasFound);
    }
}