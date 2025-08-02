using IdelPog.Flows;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Skill;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace Integration.Tests
{
    public class ManagedBuffer
    {
        protected IBufferMessenger BufferMessenger { get; private set; }
        protected IBufferManager BufferManager { get; private set; }
        protected IDispatchOne<FlowDescriptor> FlowDescriptorDispatcher { get; private set; }
        protected ICurrentSkillProvider CurrentSkillProvider;
        private IBufferFactory _bufferFactory;
        private IObjectNullAssertion _objectNullAssertion;

        [OneTimeSetUp]
        protected void BaseOneTimeSetUp()
        {
            _objectNullAssertion = new ObjectNullAssertion(new ThrowHandler());
        }

        [SetUp]
        protected void BaseSetUp()
        {
            IListenerAssertion listenerAssertion = new ListenerAssertion(new ThrowHandler());
            IBufferAssertion bufferAssertion = new BufferAssertion(new ThrowHandler());

            BufferMessenger = new BufferMessenger(_objectNullAssertion, listenerAssertion);
            _bufferFactory = new BufferFactory(bufferAssertion, _objectNullAssertion, (IBufferDispatcher) BufferMessenger);
            BufferManager = new BufferManager(_bufferFactory, _objectNullAssertion);
            
            FlowDescriptorDispatcher = new ManagedDispatcher<FlowDescriptor>(BufferManager, _objectNullAssertion, new CollectionAssertion(new ThrowHandler()));
            
            CurrentSkillProvider currentSkillProvider = new();
            ICurrentSkillSetter currentSkillSetter = currentSkillProvider;
            ICurrentSkillProvider skillProvider = currentSkillProvider;
            CurrentSkillProvider = skillProvider;
            
            FlowBootstrapper.Initialize(BufferMessenger);
            CurrencyBootstrapper.InitializeFlows(BufferManager, FlowDescriptorDispatcher);
            SkillBootstrapper.RegisterSkillChange(BufferManager, FlowDescriptorDispatcher, currentSkillSetter);
            FlowBootstrapper.InitializeFlows(BufferMessenger);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            BufferMessenger.Subscribe(listener);
        }
    }
}