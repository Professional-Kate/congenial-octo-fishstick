using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Assertions
{
    public interface IThrowingAssertion
    {
        public void AssertDoesNotThrow<TMessage>(TMessage message, ISingleController controller);
        
        public void AssertDoesNotThrow<TMessage>(IReadOnlyList<TMessage> message, IBatchedController controller);
        
    }
}