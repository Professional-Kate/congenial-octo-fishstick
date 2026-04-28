using IdelPog.Combat;
using IdelPog.Core.Contracts;
using IdelPog.Core.Flows;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Factory;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Exceptions;
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
        private BufferMessenger _bufferMessenger;
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
            Register(new CombatOptions { MaxIterations = 10000 });
        }
        
        private void Setup()
        {
            IListenerAssertion listenerAssertion = new ListenerAssertion();
            IBufferAssertion bufferAssertion = new BufferAssertion();

            _bufferMessenger = new BufferMessenger(_objectNullAssertion, listenerAssertion);
            _bufferFactory = new BufferFactory(bufferAssertion, _objectNullAssertion, _bufferMessenger);
            BufferManager = new BufferManager(_bufferFactory, _objectNullAssertion);
        }

        private void Register(CombatOptions combatOptions)
        {
            FlowRegister flowRegister = FlowBootstrapper.CreateFlowRegister(BufferManager);
            CurrencyBootstrapper.RegisterFlows(BufferManager, flowRegister);
            SkillBootstrapper.RegisterFlows(BufferManager, flowRegister);
            ContentEngineBootstrapper.RegisterFlows(BufferManager, flowRegister);
            InventoryBootstrapper.RegisterFlows(BufferManager, flowRegister);
            CombatBootstrapper.RegisterFlows(BufferManager, flowRegister, combatOptions);
            FlowBootstrapper.SubscribeFlows(flowRegister, _bufferMessenger);
        }

        protected void RegisterWithOptions(CombatOptions combatOptions)
        {
            Setup();
            Register(combatOptions);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            _bufferMessenger.Subscribe(listener);
        }

        protected void DispatchMessage<TMessage>(params TMessage[] messages) where TMessage : struct
        {
            IBuffer<TMessage> buffer = BufferManager.RequestBuffer<TMessage>(new BufferRequest(messages.Length));
            buffer.Assign(messages);
            buffer.MarkReady();
        }

        protected static void AssertBaseError<TException>(BaseError baseError)
        {
            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.GetBaseException(), Is.TypeOf<TException>());
            });
        }
    }
}