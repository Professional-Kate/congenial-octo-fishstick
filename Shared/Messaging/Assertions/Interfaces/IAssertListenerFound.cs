using IdelPog.Messaging.Messaging;

namespace IdelPog.Messaging.Assertions
{
    public interface IAssertListenerFound
    {
        public void AssertFound(IListener listener, bool wasFound);
    }
}