using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Core.Messaging.Assertion.Interface
{
    public interface IBatchControllerExecutionAssertion<T>
    {
        public void AssertBatchExecutesWithoutError(IBatchController<T> controller, IReadOnlyList<T> messages);
    }
}