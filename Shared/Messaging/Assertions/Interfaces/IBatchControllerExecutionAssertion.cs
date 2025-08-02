using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.Messaging.Assertions
{
    public interface IBatchControllerExecutionAssertion<T>
    {
        public void AssertBatchExecutesWithoutError(IBatchController<T> controller, IReadOnlyList<T> messages);
    }
}