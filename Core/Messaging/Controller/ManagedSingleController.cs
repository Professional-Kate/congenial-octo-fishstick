using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Core.Messaging.Controller
{
    public sealed class ManagedSingleController<T> : ISingleController<T> where T : struct
    {
        private readonly ISingleMediator<T> _singleMediator;

        public ManagedSingleController(ISingleMediator<T> singleMediator)
        {
            _singleMediator = singleMediator;
        }

        public void HandleMessage(T message)
        {
            _singleMediator.HandleMessage(message);
        }
    }
}