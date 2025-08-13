using IdelPog.Core.Messaging.Assertion.Interface;

namespace IdelPog.Core.Messaging.Listener.Single
{
    public sealed class ManagedSingleListener<T> : ISingleListener<T>
    {
        private readonly ISingleController<T> _controller;
        private readonly ISingleControllerExecutionAssertion<T> _singleControllerExecutionAssertion;

        public ManagedSingleListener(ISingleController<T> controller, ISingleControllerExecutionAssertion<T> singleControllerExecutionAssertion)
        {
            _controller = controller;
            _singleControllerExecutionAssertion = singleControllerExecutionAssertion;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(T message)
        {
            _singleControllerExecutionAssertion.AssertExecutesWithoutError(_controller, message);
        }
    }
}