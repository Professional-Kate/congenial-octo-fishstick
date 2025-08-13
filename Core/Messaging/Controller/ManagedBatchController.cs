using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Core.Messaging.Controller
{
    public sealed class ManagedBatchController<T> : IBatchController<T> where T : struct
    {
        private readonly IBatchMediator<T> _batchMediator;

        public ManagedBatchController(IBatchMediator<T> batchMediator)
        {
            _batchMediator = batchMediator;
        }

        public void HandleMessages(IReadOnlyList<T> messages)
        {
            _batchMediator.HandleMessages(messages);
        }
    }
}