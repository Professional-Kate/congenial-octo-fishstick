using IdelPog.Flows.Builder;

namespace IdelPog.Flows.Service
{
    public interface IFlowConstructionService
    {
        public void ConstructAndSubscribe<TCommand, TError>(FlowDescriptor flowDescriptor);
    }
}