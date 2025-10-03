using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Exceptions;
using IdelPog.Progression.Runtime.Component;

// ReSharper disable ObjectCreationAsStatement

namespace IdelPog.Progression.Tests.Component
{
    [TestFixture]
    public sealed class QueueComponentStoreTest
    {
        private QueueComponentStore<TestComponent> _queueComponentStore;
        private IHandler _handler;
        private const int AMOUNT_OF_COMPONENTS = 10;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _handler = new ThrowHandler();
        }
        
        [SetUp]
        public void Setup()
        {
            TestComponent[] testComponents = new TestComponent[AMOUNT_OF_COMPONENTS];
            for (int i = 0; i < AMOUNT_OF_COMPONENTS; i++)
            {
                testComponents[i] = CreateComponent(i);
            }

            _queueComponentStore = new QueueComponentStore<TestComponent>(testComponents, _handler);
        }

        private static TestComponent CreateComponent(int index)
        {
            return new TestComponent { Index = index };
        }

        private static void AssertComponent(TestComponent component, int expectedIndex)
        { 
            Assert.That(component.Index, Is.EqualTo(expectedIndex));
        }

        [Test]
        public void Positive_Peek_GetsFrontComponent()
        {
            TestComponent frontComponent = _queueComponentStore.Peek();
            
            AssertComponent(frontComponent, 0);
        }
        
        [Test]
        public void Positive_TryDequeue_DequeuesFrontComponent()
        {
            bool successful = _queueComponentStore.TryDequeue(out TestComponent frontComponent);
            
            Assert.Multiple(() =>
            {
                Assert.That(successful, Is.True);
                AssertComponent(frontComponent, 0);
                AssertComponent(_queueComponentStore.Peek(), 1);
            });
        }

        [Test]
        public void Positive_TryDequeue_CanDequeueEverything()
        {
            for (int i = 0; i < AMOUNT_OF_COMPONENTS; i++)
            {
                bool successful = _queueComponentStore.TryDequeue(out TestComponent frontComponent);
                Assert.That(successful, Is.True);
                AssertComponent(frontComponent, i);
            }
        }
        
        [Test]
        public void Positive_TryDequeue_DequeueEverything_TryDequeue_ReturnsFalse()
        {
            for (int i = 0; i < AMOUNT_OF_COMPONENTS; i++)
            {
                _queueComponentStore.TryDequeue(out TestComponent _);
            }
            
            bool successful = _queueComponentStore.TryDequeue(out TestComponent _);
            Assert.That(successful, Is.False);
        }

        [Test]
        public void Positive_ToArray_ReturnsExpectedAmount()
        {
            TestComponent[] components = _queueComponentStore.ToArray();
            
            Assert.That(components, Has.Length.EqualTo(AMOUNT_OF_COMPONENTS));
        }

        [Test]
        public void Positive_DeepClone_ReturnsClone()
        { 
            QueueComponentStore<TestComponent> clonedStore =  _queueComponentStore.DeepClone();
            clonedStore.TryDequeue(out TestComponent testComponent);
            
            AssertComponent(testComponent, 0);
            AssertComponent(_queueComponentStore.Peek(), 0);
        }

        [Test]
        public void Negative_PeekAfterEmpty_Throws()
        {
            for (int i = 0; i < AMOUNT_OF_COMPONENTS; i++)
            {
                _queueComponentStore.TryDequeue(out TestComponent _);
            }
            
            Assert.Throws<InvalidOperationException>(() => _queueComponentStore.Peek());
        }

        [Test]
        public void Negative_EmptyComponents_Throws()
        {
            Assert.Throws<ComponentArrayEmptyException>(() => new QueueComponentStore<TestComponent>([], _handler));
        }
        
        [Test]
        public void Negative_NullComponents_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new QueueComponentStore<TestComponent>(null!, _handler));
        }
    }
}