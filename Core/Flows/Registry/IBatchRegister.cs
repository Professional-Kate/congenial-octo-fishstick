using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Core.Flows.Registry
{
    public interface IBatchRegister
    {
        public void RegisterBatch<TCommand>(IBatchMediator<TCommand> mediator) where TCommand : struct;
    }
}