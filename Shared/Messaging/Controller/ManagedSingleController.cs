using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Controller
{
    public sealed class ManagedSingleController<T> : ISingleController<T>
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