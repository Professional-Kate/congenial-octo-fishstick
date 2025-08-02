using IdelPog.Messaging.Assertions;

namespace IdelPog.Messaging.Listeners.Single
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
        
        public void Handle(T item)
        {
            _singleControllerExecutionAssertion.AssertExecutesWithoutError(_controller, item);
        }
    }
}