using IdelPog.ECS.Component;
using IdelPog.ECS.Component.Store;

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public class ComponentStoreTest
    {
        private ComponentStore<TestComponent> _componentStore;
        private TestComponent _testComponent;

        [SetUp]
        public void SetUp()
        {
            _testComponent = new TestComponent();
            _componentStore = new ComponentStore<TestComponent>();
        }
        
        [Test]
        public void Positive_AddComponent_StoresComponent()
        {
            _componentStore.AddComponent(_testComponent);

            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(1));
        }

        [Test]
        public void Positive_AddComponent_MultipleAdds_StoresMultiple()
        {
            _componentStore.AddComponent(_testComponent);
            _componentStore.AddComponent(new TestComponent());
            _componentStore.AddComponent(new TestComponent());
            
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(3));
        }

        [Test]
        public void Negative_AddComponent_TwiceSameComponent_Throws()
        {
            _componentStore.AddComponent(_testComponent);
            
            Assert.Throws<Exception>(() => _componentStore.AddComponent(_testComponent));
        }

        [Test]
        public void Positive_RemoveComponent_RemovesStoredComponent()
        {
            _componentStore.AddComponent(_testComponent);
            _componentStore.RemoveComponent(_testComponent);
            
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_RemoveComponent_RemovesCorrectComponent()
        {
            TestComponent testComponentOne = new() { TestNumber = 1 };
            TestComponent testComponentTwo = new() { TestNumber = 2 };
            
            _componentStore.AddComponent(testComponentOne);
            _componentStore.AddComponent(testComponentTwo);
            
            _componentStore.RemoveComponent(testComponentTwo);
            
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(1));
            Assert.That(testComponents[0].TestNumber, Is.EqualTo(1));
        }

        [Test]
        public void Negative_RemoveComponent_ComponentNotFound_Throws()
        {
            Assert.Throws<Exception>(() => _componentStore.RemoveComponent(_testComponent));
        }

        [Test]
        public void Positive_GetAllComponents_ReturnsAllComponents()
        {
            _componentStore.AddComponent(_testComponent);
            _componentStore.AddComponent(new TestComponent());
            
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(2));
        }

        [Test]
        public void Positive_GetAllComponents_ReturnsReference_OriginalInstanceIsMutated()
        {
            TestComponent testComponent = new() { TestNumber = 1 };
            _componentStore.AddComponent(testComponent);
            
            // To test returning by reference we must first add, then get that pointer back
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(1));

            // Then we locally assign the TestNumber to 4 which should change the number in the ComponentStore
            testComponents[0].TestNumber = 4;
            
            // We get that pointer again and make sure it is 4
            TestComponent[] testComponentsAgain = _componentStore.GetAllComponents();
            Assert.Multiple(() =>
            {
                Assert.That(testComponentsAgain, Has.Length.EqualTo(1));
                Assert.That(testComponentsAgain[0].TestNumber, Is.EqualTo(4));
                // sanity check making sure we get back the exact same reference 
                Assert.That(ReferenceEquals(testComponents[0], testComponentsAgain[0]), Is.True);
            });
        }

        [Test]
        public void Positive_GetAllComponents_EmptyComponents_ReturnsEmptyList()
        {
            TestComponent[] testComponents = _componentStore.GetAllComponents();
            Assert.That(testComponents, Has.Length.EqualTo(0));
        }
    }
}