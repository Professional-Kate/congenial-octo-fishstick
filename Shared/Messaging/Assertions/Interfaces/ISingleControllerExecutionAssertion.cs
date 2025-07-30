using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Assertions
{
    public interface ISingleControllerExecutionAssertion<TContext>
    {
        public void AssertExecutesWithoutError(ISingleController<TContext> controller, TContext message);
    }
}