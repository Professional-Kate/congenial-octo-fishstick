using IdelPog.Core.Flows;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer.Factory;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Currency;
using IdelPog.HarvestNode;
using IdelPog.HarvestNode.Services;
using IdelPog.Inventory;
using IdelPog.Skill;
using IdelPog.Skill.Service;

namespace IdelPog.Integration.Tests
{
    public class ManagedBuffer
    {
        protected IBufferManager BufferManager { get; private set; }
        protected ICurrentSkillProvider CurrentSkillProvider;
        protected ICurrentHarvestTargetProvider CurrentHarvestTargetProvider;
        private IBufferMessenger _bufferMessenger { get; set; }
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
        }

        private void Register()
        {
            CurrentSkillProvider skillProvider = new();
            ICurrentSkillSetter skillSetter = skillProvider;
            CurrentSkillProvider = skillProvider;

            CurrentHarvestTargetProvider harvestTargetProvider = new();
            CurrentHarvestTargetProvider = harvestTargetProvider;

            FlowRegister flowRegister = FlowBootstrapper.CreateFlowRegister(BufferManager);
            CurrencyBootstrapper.RegisterFlows(BufferManager, flowRegister);
            SkillBootstrapper.RegisterSetSkill(BufferManager, skillSetter, flowRegister);
            ContentEngineBootstrapper.RegisterFlows(BufferManager, harvestTargetProvider, flowRegister, flowRegister);
            InventoryBootstrapper.RegisterInventoryUpdate(BufferManager, flowRegister);
            FlowBootstrapper.SubscribeFlows(flowRegister, _bufferMessenger);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            _bufferMessenger.Subscribe(listener);
        }
    }
}