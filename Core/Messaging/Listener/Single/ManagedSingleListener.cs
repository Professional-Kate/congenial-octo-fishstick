using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Contracts;
using IdelPog.Core.Messaging.Assertion.Interface;

namespace IdelPog.Core.Messaging.Listener.Single
{
    public sealed class ManagedSingleListener<T> : ISingleListener<T> where T : struct
    {
        private readonly ISingleController<T> _controller;
        private readonly ISingleControllerExecutionAssertion<T> _singleControllerExecutionAssertion;
        private readonly ILogger _logger;

        public ManagedSingleListener(ISingleController<T> controller, ISingleControllerExecutionAssertion<T> singleControllerExecutionAssertion, ILogger logger)
        {
            _controller = controller;
            _singleControllerExecutionAssertion = singleControllerExecutionAssertion;
            _logger = logger;
        }

        public Type ListenerType => typeof(T);
        
        public void Handle(T message)
        {
            _logger.Log(LogLevel.INFO, LogDirection.IN, message);
            _singleControllerExecutionAssertion.AssertExecutesWithoutError(_controller, message);
        }
    }
}