using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Core.Flows.Registry
{
    public interface ISingleRegister
    {
        public void Register<TCommand, TError>(ISingleController<TCommand> controller, IErrorFactory<TError, TCommand> factory) where TCommand : struct where TError : struct;
    }
}