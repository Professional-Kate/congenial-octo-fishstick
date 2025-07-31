using IdelPog.Flows.Builder;
using IdelPog.Messaging.Listeners;

namespace IdelPog.Flows.Register
{
    public interface IFlowRegister
    {
        public IListener ConstructFlow<TCommand, TError>(DispatchMode dispatchMode);
    }
}