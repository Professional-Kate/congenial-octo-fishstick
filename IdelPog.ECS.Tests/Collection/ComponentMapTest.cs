using IdelPog.ECS.Collection;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Tests.Collection
{
    [TestFixture]
    public class ComponentMapTest
    {
        private ComponentMap _componentMap { get; set; }
        private TestComponent _testComponent { get; set; }
        private ComponentStore<TestComponent> _componentStore { get; set; }

        [SetUp]
        public void Setup()
        {
            _componentMap = new ComponentMap();
            _componentStore = new ComponentStore<TestComponent>([_testComponent], new ThrowHandler());
            _testComponent = new TestComponent { TestNumber = 1 };
        }

        private void AssertMapContains(IComponent component, bool expectedContains)
        {
            Assert.That(_componentMap.Contains(component), Is.EqualTo(expectedContains));
        }
        
        [Test]
        public void Positive_Add_AddsComponent()
        {
            _componentMap.Add(_testComponent);

            AssertMapContains(_testComponent, true);
        }

        [Test]
        public void Positive_Add_AddingMultipleDifferentComponents()
        {
            _componentMap.Add(_testComponent);
            _componentMap.Add(_componentStore);
            
            AssertMapContains(_testComponent, true);
            AssertMapContains(_componentStore, true);
        }

        [Test]
        public void Positive_AddArray_AddsEntireArray()
        {
            _componentMap.Add([_testComponent, _componentStore]);
            
            AssertMapContains(_testComponent, true);
            AssertMapContains(_componentStore, true);
        }
        
        [Test]
        public void Positive_Remove_RemovesAddedComponent()
        {
            _componentMap.Add(_testComponent);
            _componentMap.Remove<TestComponent>();
            
            AssertMapContains(_testComponent, false);
        }

        [Test]
        public void Positive_Remove_RemovesCorrectComponent()
        {
            _componentMap.Add(_testComponent);
            _componentMap.Add(_componentStore);
            _componentMap.Remove<TestComponent>();
            
            AssertMapContains(_testComponent, false);
            AssertMapContains(_componentStore, true);
        }

        [Test]
        public void Positive_Get_RetrievesCorrectComponent()
        {
            _componentMap.Add(_testComponent);
            _componentMap.Add(_componentStore);
            
            IComponent component = _componentMap.Get<TestComponent>();
            Assert.That(component, Is.Not.Null);
            Assert.That(component, Is.TypeOf<TestComponent>());
        }

        [Test]
        public void Positive_ContainsType_ReturnsTrue()
        {
            _componentMap.Add(_testComponent);
            
            bool contains = _componentMap.Contains<TestComponent>();
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Positive_ContainsType_ReturnsFalse()
        {
            _componentMap.Add(_testComponent);
            
            bool contains = _componentMap.Contains<ComponentStore<TestComponent>>();
            Assert.That(contains, Is.False);
        }
        
        [Test]
        public void Positive_ContainsIComponent_ReturnsTrue()
        {
            _componentMap.Add(_componentStore);
            
            bool contains = _componentMap.Contains(_componentStore);
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Positive_ContainsIComponent_ReturnsFalse()
        {
            _componentMap.Add(_componentStore);
            
            bool contains = _componentMap.Contains(_testComponent);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Negative_Add_ComponentNotFound_Throws()
        {
            _componentMap.Add(_testComponent);
            Assert.Throws<Exception>(() => _componentMap.Add(_testComponent));
        }

        [Test]
        public void Negative_Add_NullComponent_Throws()
        {
            IComponent? component = null;
            Assert.Throws<Exception>(() => _componentMap.Add(component!));
        }

      
        [Test]
        public void Negative_AddArray_EmptyArray_Throws()
        {
            Assert.Throws<Exception>(() => _componentMap.Add([]));
        }
        
        [Test]
        public void Negative_AddArray_NullArray_Throws()
        {
            Assert.Throws<Exception>(() => _componentMap.Add([null!, null!]));
        }

        [Test]
        public void Negative_AddArray_DuplicateComponents_Throws()
        {
            Assert.Throws<Exception>(() => _componentMap.Add([_testComponent, _componentStore, _testComponent]));
        }

        [Test]
        public void Negative_Remove_ComponentNotFound_Throws()
        {
            _componentMap.Add(_componentStore);
            
            Assert.Throws<Exception>(() => _componentMap.Remove<TestComponent>());
        }
        
        [Test]
        public void Negative_Get_TypeNotFound_Throws()
        {
            Assert.Throws<Exception>(() => _componentMap.Get<TestComponent>());
        }
    }
}