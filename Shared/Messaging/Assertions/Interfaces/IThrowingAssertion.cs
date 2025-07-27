using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Assertions
{
    public interface IThrowingAssertion
    {
        public void AssertDoesNotThrow<TMessage>(TMessage message, ISingleController<TMessage> controller);
        
        public void AssertDoesNotThrow<TMessage>(IReadOnlyList<TMessage> message, IBatchedController<TMessage> controller);
        
    }
}