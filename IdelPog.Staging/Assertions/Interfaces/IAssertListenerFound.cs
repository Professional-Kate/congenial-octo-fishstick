using IdelPog.Staging.Messaging;

namespace IdelPog.Staging.Assertions
{
    public interface IAssertListenerFound
    {
        public void AssertFound(IListener listener, bool wasFound);
    }
}