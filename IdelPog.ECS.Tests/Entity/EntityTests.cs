using IdelPog.ECS.Collection;
using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Infrastructure.Structures;
using IdelPog.Validation.Assertions.Handlers;
using Moq;
using NUnit.Framework.Internal;

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public class EntityTests
    {
        private TestEntity _entity { get; set; }
        private Mock<IComponentMap> _componentMapMock { get; set; }
        private TestComponent _testComponent { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testComponent = new TestComponent();
            _componentMapMock = new Mock<IComponentMap>();
            _handlerMock = new Mock<IHandler>();
            _entity = new TestEntity(_componentMapMock.Object, _handlerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _componentMapMock.Reset();
        }

        [Test]
        public void Positive_AddRequiredComponents_AddsComponents()
        {
            _componentMapMock.Setup(library => library.Contains<TestComponent>())
                .Returns(true);
            
            _componentMapMock.Setup(library => library.Get<TestComponent>())
                .Returns(_testComponent);
            
            Optional<TestComponent> maybeComponent = _entity.TryGetComponent<TestComponent>();
            Assert.Multiple(() =>
            {
                Assert.That(maybeComponent.HasValue, Is.True);
                Assert.That(maybeComponent.GetValue(), Is.EqualTo(_testComponent));
            });
        }

        [Test]
        public void Positive_AddComponent_AddsComponent()
        {
            _componentMapMock.Setup(library => library.Contains(It.Is<IComponent>(type => ReferenceEquals(type, _testComponent.GetType()))))
                .Returns(false);
            
            _entity.AddComponent(_testComponent);
            
            _componentMapMock.Verify(library => library.Contains(_testComponent), Times.Once);
            _componentMapMock.Verify(library => library.Add(_testComponent), Times.Once);
        }

        [Test]
        public void Negative_AddComponent_DuplicatesComponent_Throws()
        {
            _componentMapMock.Setup(library => library.Contains(_testComponent))
                .Returns(true);
            
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentAlreadyExistsException>()))
                .Throws(new ComponentAlreadyExistsException(_testComponent));
            
            Assert.Throws<ComponentAlreadyExistsException>(() => _entity.AddComponent(_testComponent));
            
            _componentMapMock.Verify(library => library.Contains(_testComponent), Times.Once);
            _componentMapMock.Verify(library => library.Add(_testComponent), Times.Never);
        }

        [Test]
        public void Positive_RemoveComponent_RemovesComponent()
        {
            _componentMapMock.Setup(library => library.Contains<TestComponent>())
                .Returns(true);
            
            _entity.RemoveComponent<TestComponent>();
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Remove<IComponent>(), Times.Once);
        }

        [Test]
        public void Negative_RemoveComponent_MissingComponent_Throws()
        {
            _componentMapMock.Setup(library => library.Contains(_testComponent))
                .Returns(false);

            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentNotFoundException>()))
                .Throws(new ComponentNotFoundException(typeof(TestComponent)));

            Assert.Throws<ComponentNotFoundException>(() => _entity.RemoveComponent<TestComponent>());
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Remove<IComponent>(), Times.Never);
        }

        [Test]
        public void Positive_GetComponent_ReturnsComponent()
        {
            _componentMapMock.Setup(library => library.Contains<TestComponent>())
                .Returns(true);
            
            _componentMapMock.Setup(library => library.Get<TestComponent>())
                .Returns(_testComponent);
            
            IComponent component = _entity.GetComponent<TestComponent>();
            
            Assert.That(component, Is.EqualTo(_testComponent));
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Get<TestComponent>(), Times.Once);
        }

        [Test]
        public void Negative_GetComponent_NotFound_Throws()
        {
            _componentMapMock.Setup(library => library.Contains(_testComponent))
                .Returns(false);
            
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentNotFoundException>()))
                .Throws(new ComponentNotFoundException(typeof(TestComponent)));
            
            Assert.Throws<ComponentNotFoundException>(() => _entity.GetComponent<TestComponent>());
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Get<IComponent>(), Times.Never);
        }

        [Test]
        public void Positive_TryGetComponent_GetsComponent_ReturnsTrue()
        {
            _componentMapMock.Setup(library => library.Contains<TestComponent>())
                .Returns(true);
            
            _componentMapMock.Setup(library => library.Get<IComponent>())
                .Returns(_testComponent);
            
            Optional<TestComponent> contains = _entity.TryGetComponent<TestComponent>();
            Assert.Multiple(() =>
            {
                Assert.That(contains.HasValue, Is.True);
                Assert.That(contains.GetValue(), Is.EqualTo(_testComponent));
            });
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Get<IComponent>(), Times.Once);
        }

        [Test]
        public void Negative_TryGetComponent_NotFound_ReturnsFalse()
        {
            _componentMapMock.Setup(library => library.Contains<TestComponent>())
                .Returns(false);
            
            Optional<TestComponent> contains = _entity.TryGetComponent<TestComponent>();
            Assert.That(contains.HasValue, Is.False);
            
            _componentMapMock.Verify(library => library.Contains<TestComponent>(), Times.Once);
            _componentMapMock.Verify(library => library.Get<IComponent>(), Times.Never);
        }
    }
}