using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public class ImmutableComponentStoreTest
    {
        private ImmutableComponentStore<TestComponent> _componentStore;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SetupComponentStoreWith(10);
        }

        private void SetupComponentStoreWith(int count)
        {
            TestComponent[] components = new TestComponent[count];

            for (int i = 0; i < count; i++)
            {
                components[i] = new TestComponent { TestNumber = i };
            }
            
            _componentStore = new ImmutableComponentStore<TestComponent>(components);
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

            // This should only change this local TestComponent. Not the one in the store 
            testComponents[0].TestNumber = 100;
            
            TestComponent[] testComponentsAgain = _componentStore.GetAllComponents();
            Assert.That(testComponentsAgain, Has.Length.EqualTo(1));
            
            // After getting again we ensure the number has not changed
            Assert.That(testComponentsAgain[0].TestNumber, Is.EqualTo(originalNumber));
        }

        [Test]
        public void Negative_ConstructNewStore_EmptyArray_Throws()
        {
            Assert.Throws<Exception>(() => new ImmutableComponentStore<TestComponent>([]));
        }
        
        [Test]
        public void Negative_ConstructNewStore_NullArray_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ImmutableComponentStore<TestComponent>(null!));
        }
    }
}