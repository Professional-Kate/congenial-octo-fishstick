using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Core.Flows.Registry
{
    public interface IBatchRegister
    {
        public void Register<TCommand, TError>(IBatchController<TCommand> controller, IErrorFactory<TError, IReadOnlyList<TCommand>> factory) where TCommand : struct where TError : struct;
    }
}