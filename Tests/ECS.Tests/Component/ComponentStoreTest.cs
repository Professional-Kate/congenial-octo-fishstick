using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;

// ReSharper disable ObjectCreationAsStatement

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public sealed class ComponentStoreTest
    {
        private ComponentStore<TestComponent> _componentStore;

        private void SetupComponentStoreWith(int count)
        {
            TestComponent[] components = new TestComponent[count];

            for (int i = 0; i < count; i++)
            {
                components[i] = new TestComponent { TestNumber = i };
            }

            _componentStore = new ComponentStore<TestComponent>(components);
        }

        [Test]
        public void Positive_GetAllComponents_ReturnsAllComponents()
        {
            SetupComponentStoreWith(10);
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
        public void Positive_ContainsComponent_ContainsComponent_ShouldReturnsTrue()
        {
            SetupComponentStoreWith(1);

            bool contains = _componentStore.ContainsComponent(component => component.TestNumber == 0);

            Assert.That(contains, Is.True);
        }

        [Test]
        public void Positive_ContainsComponent_DoesNotContainsComponent_ShouldReturnsFalse()
        {
            SetupComponentStoreWith(1);

            bool contains = _componentStore.ContainsComponent(component => component.TestNumber == 10);

            Assert.That(contains, Is.False);
        }

        [Test]
        public void Negative_ConstructNewStore_EmptyArray_Throws()
        {
            Assert.Throws<ComponentArrayEmptyException>(() => new ComponentStore<TestComponent>([]));
        }

        [Test]
        public void Negative_ConstructNewStore_NullArray_Throws()
        {
            Assert.Throws<ComponentArrayNullException>(() => new ComponentStore<TestComponent>(null!));
        }

        [Test]
        public void Positive_CloneComponent_Clones()
        {
            SetupComponentStoreWith(1);
            ComponentStore<TestComponent> clonedStore = _componentStore.DeepClone();

            Assert.That(clonedStore.GetAllComponents(), Is.EqualTo(_componentStore.GetAllComponents()));
        }
    }
}