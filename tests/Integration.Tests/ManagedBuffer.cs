using ContentEngine;
using ContentEngine.Services;
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
        protected IBufferManager BufferManager { get; private set; }
        protected ICurrentSkillProvider CurrentSkillProvider;
        private IBufferMessenger _bufferMessenger { get; set; }
        private IDispatchOne<FlowDescriptor> _flowDescriptorDispatcher { get; set; }
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
            Setup();
            Register();
        }
        
        private void Setup()
        {
            IListenerAssertion listenerAssertion = new ListenerAssertion(new ThrowHandler());
            IBufferAssertion bufferAssertion = new BufferAssertion(new ThrowHandler());

            _bufferMessenger = new BufferMessenger(_objectNullAssertion, listenerAssertion);
            _bufferFactory = new BufferFactory(bufferAssertion, _objectNullAssertion, (IBufferDispatcher)_bufferMessenger);
            BufferManager = new BufferManager(_bufferFactory, _objectNullAssertion);

            _flowDescriptorDispatcher = new ManagedDispatcher<FlowDescriptor>(BufferManager, _objectNullAssertion, new CollectionAssertion(new ThrowHandler()));
        }

        private void Register()
        {
            CurrentSkillProvider skillProvider = new();
            ICurrentSkillSetter skillSetter = skillProvider;
            CurrentSkillProvider = skillProvider;

            CurrentResourceProvider resourceProvider = new();

            FlowBootstrapper.Initialize(_bufferMessenger);
            CurrencyBootstrapper.RegisterFlows(BufferManager, _flowDescriptorDispatcher);
            SkillBootstrapper.RegisterSetSkill(BufferManager, _flowDescriptorDispatcher, skillSetter);
            ContentEngineBootstrapper.RegisterFlows(BufferManager, _flowDescriptorDispatcher, resourceProvider);
            FlowBootstrapper.InitializeFlows(_bufferMessenger);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            _bufferMessenger.Subscribe(listener);
        }
    }
}