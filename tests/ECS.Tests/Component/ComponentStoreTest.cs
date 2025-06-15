using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public class ComponentStoreTest
    {
        private ComponentStore<TestComponent> _componentStore;
        private Mock<IHandler> _handlerMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            SetupComponentStoreWith(10);
        }

        private void SetupComponentStoreWith(int count)
        {
            TestComponent[] components = new TestComponent[count];

            for (int i = 0; i < count; i++)
            {
                components[i] = new TestComponent { TestNumber = i };
            }
            
            _componentStore = new ComponentStore<TestComponent>(components, _handlerMock.Object);
        }

        [Test]
        public void Positive_GetAllComponents_ReturnsAllComponents()
        {
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            
            Assert.That(testComponents, Has.Length.EqualTo(10));
        }

        [Test]
        public void Positive_GetAllComponents_ReturnsClones_NotReferences()
        {
            SetupComponentStoreWith(1);
            
            // Get the component, store the number for later, and then set the component TestNumber to 100
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(1));
            int originalNumber = testComponents[0].TestNumber;
            
            TestComponent[] testComponentsAgain = _componentStore.GetAllComponents();
            Assert.That(testComponentsAgain, Has.Length.EqualTo(1));
            
            // After getting again we ensure the number has not changed
            Assert.That(testComponentsAgain[0].TestNumber, Is.EqualTo(originalNumber));
        }

        [Test]
        public void Negative_ConstructNewStore_EmptyArray_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentArrayEmptyException>()))
                .Throws(new ComponentArrayEmptyException());
            
            Assert.Throws<ComponentArrayEmptyException>(() => new ComponentStore<TestComponent>([], _handlerMock.Object));
        }
        
        [Test]
        public void Negative_ConstructNewStore_NullArray_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentArrayNullException>()))
                .Throws(new ComponentArrayNullException());
            
            Assert.Throws<ComponentArrayNullException>(() => new ComponentStore<TestComponent>(null!, _handlerMock.Object));
        }

        [Test]
        public void Positive_CloneComponent_Clones()
        {
            ComponentStore<TestComponent> clonedStore = _componentStore.DeepClone();
            
            Assert.That(clonedStore, Is.Not.Null);
            Assert.That(clonedStore.GetAllComponents(), Is.EqualTo(_componentStore.GetAllComponents()));
        }
    }
}