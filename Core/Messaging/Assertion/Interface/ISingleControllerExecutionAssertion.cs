using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Core.Messaging.Assertion.Interface
{
    public interface ISingleControllerExecutionAssertion<TContext> where TContext : struct
    {
        public void AssertExecutesWithoutError(ISingleController<TContext> controller, TContext message);
    }
}