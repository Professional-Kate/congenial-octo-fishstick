using IdelPog.Common.Repository;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Validation.Assertions;

namespace IdelPog.Flows.Registry
{
    public class FlowRegistryMediator : IBatchMediator<FlowDescriptor>
    {
        private readonly IAssetRepository<Type, FlowDescriptor> _flowRepository;
        private readonly IUniqueAssertion _uniqueAssertion;

        public FlowRegistryMediator(IAssetRepository<Type, FlowDescriptor> flowRepository, IUniqueAssertion uniqueAssertion)
        {
            _flowRepository = flowRepository;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<FlowDescriptor> flowDescriptors)
        {
            foreach (FlowDescriptor flowDescriptor in flowDescriptors)
            { 
                Console.WriteLine(flowDescriptor.Description);
                _uniqueAssertion.AssertUnique(flowDescriptor.CommandType, _flowRepository.Contains(flowDescriptor.CommandType));
                _flowRepository.Add(flowDescriptor.CommandType, flowDescriptor);
            }
        }
    }
}