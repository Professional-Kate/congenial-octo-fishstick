using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Infrastructure.Repository;
using IdelPog.Infrastructure.Structures;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.ECS.Tests
{
    [TestFixture]
    public class EntityTests
    {
        private TestEntity _entity { get; set; }
        private Mock<IRepository<Type, IComponent>> _repositoryMock { get; set; }
        private TestComponent _testComponent { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testComponent = new TestComponent();
            _repositoryMock = new Mock<IRepository<Type, IComponent>>();
            _handlerMock = new Mock<IHandler>();
            _entity = new TestEntity(_repositoryMock.Object, _handlerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.Reset();
        }

        [Test]
        public void Positive_AddRequiredComponents_AddsComponents()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(true);
            
            _repositoryMock.Setup(library => library.Get(It.Is<Type>(type => type == _testComponent.GetType())))
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
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(false);
            
            _entity.AddComponent(_testComponent);
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Add(_testComponent.GetType(), _testComponent), Times.Once);
        }

        [Test]
        public void Negative_AddComponent_DuplicatesComponent_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(true);
            
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentAlreadyExistsException>()))
                .Throws(new ComponentAlreadyExistsException(_testComponent));
            
            Assert.Throws<ComponentAlreadyExistsException>(() => _entity.AddComponent(_testComponent));
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Add(_testComponent.GetType(), _testComponent), Times.Never);
        }

        [Test]
        public void Positive_RemoveComponent_RemovesComponent()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(true);
            
            _entity.RemoveComponent<TestComponent>();
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Remove(_testComponent.GetType()), Times.Once);
        }

        [Test]
        public void Negative_RemoveComponent_MissingComponent_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(false);

            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentNotFoundException>()))
                .Throws(new ComponentNotFoundException(typeof(TestComponent)));

            Assert.Throws<ComponentNotFoundException>(() => _entity.RemoveComponent<TestComponent>());
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Remove(_testComponent.GetType()), Times.Never);
        }

        [Test]
        public void Positive_GetComponent_ReturnsComponent()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(true);
            
            _repositoryMock.Setup(library => library.Get(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(_testComponent);
            
            TestComponent component = _entity.GetComponent<TestComponent>();
            
            Assert.That(component, Is.EqualTo(_testComponent));
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Get(_testComponent.GetType()), Times.Once);
        }

        [Test]
        public void Negative_GetComponent_NotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(false);
            
            _handlerMock.Setup(library => library.Handle(It.IsAny<ComponentNotFoundException>()))
                .Throws(new ComponentNotFoundException(typeof(TestComponent)));
            
            Assert.Throws<ComponentNotFoundException>(() => _entity.GetComponent<TestComponent>());
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Get(_testComponent.GetType()), Times.Never);
        }

        [Test]
        public void Positive_TryGetComponent_GetsComponent_ReturnsTrue()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(true);
            
            _repositoryMock.Setup(library => library.Get(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(_testComponent);
            
            Optional<TestComponent> contains = _entity.TryGetComponent<TestComponent>();
            Assert.Multiple(() =>
            {
                Assert.That(contains.HasValue, Is.True);
                Assert.That(contains.GetValue(), Is.EqualTo(_testComponent));
            });
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Get(_testComponent.GetType()), Times.Once);
        }

        [Test]
        public void Negative_TryGetComponent_NotFound_ReturnsFalse()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => type == _testComponent.GetType())))
                .Returns(false);
            
            Optional<TestComponent> contains = _entity.TryGetComponent<TestComponent>();
            Assert.That(contains.HasValue, Is.False);
            
            _repositoryMock.Verify(library => library.Contains(_testComponent.GetType()), Times.Once);
            _repositoryMock.Verify(library => library.Get(_testComponent.GetType()), Times.Never);
        }
    }
}