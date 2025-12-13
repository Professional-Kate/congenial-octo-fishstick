using IdelPog.Combat;
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
using IdelPog.Currency;
using IdelPog.HarvestNode;
using IdelPog.Inventory;
using IdelPog.Skill;

namespace IdelPog.Integration.Tests
{
    public class ManagedTestBuffer
    {
        protected IBufferManager BufferManager { get; private set; }
        private IBufferMessenger _bufferMessenger { get; set; }
        private IBufferFactory _bufferFactory;
        private IObjectNullAssertion _objectNullAssertion;

        [OneTimeSetUp]
        protected void BaseOneTimeSetUp()
        {
            _objectNullAssertion = new ObjectNullAssertion();
        }

        [SetUp]
        protected void BaseSetUp()
        {
            Setup();
            Register();
        }
        
        private void Setup()
        {
            IListenerAssertion listenerAssertion = new ListenerAssertion();
            IBufferAssertion bufferAssertion = new BufferAssertion();

            _bufferMessenger = new BufferMessenger(_objectNullAssertion, listenerAssertion);
            _bufferFactory = new BufferFactory(bufferAssertion, _objectNullAssertion, (IBufferDispatcher)_bufferMessenger);
            BufferManager = new BufferManager(_bufferFactory, _objectNullAssertion);
        }

        private void Register()
        {
            FlowRegister flowRegister = FlowBootstrapper.CreateFlowRegister(BufferManager);
            CurrencyBootstrapper.RegisterFlows(BufferManager, flowRegister);
            SkillBootstrapper.RegisterFlows(BufferManager, flowRegister);
            ContentEngineBootstrapper.RegisterFlows(BufferManager, flowRegister);
            InventoryBootstrapper.RegisterFlows(BufferManager, flowRegister);
            CombatBootstrapper.RegisterFlows(BufferManager, flowRegister);
            FlowBootstrapper.SubscribeFlows(flowRegister, _bufferMessenger);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            _bufferMessenger.Subscribe(listener);
        }
    }
}