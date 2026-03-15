using IdelPog.Core.Repository.Asset;
using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using Moq;

namespace IdelPog.ECS.Tests.Entity
{
    [TestFixture]
    public sealed class EntityTests
    {
        private ECS.Entity.Entity _entity { get; set; }
        private Mock<IAssetRepository<Type, IComponent>> _repositoryMock { get; set; }
        private TestComponent _testComponent { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testComponent = new TestComponent();
            _repositoryMock = new Mock<IAssetRepository<Type, IComponent>>();
            _entity = new TestEntity(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.Reset();
        }

        [Test]
        public void Positive_AddRequiredComponents_AddsComponents()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            _repositoryMock.Setup(library => library.Get(typeof(TestComponent)))
                .Returns(_testComponent);

            bool contains = _entity.TryGetComponent(out TestComponent component);
            Assert.Multiple(() =>
            {
                Assert.That(contains, Is.True);
                Assert.That(component, Is.EqualTo(_testComponent));
            });
        }

        [Test]
        public void Positive_AddComponent_AddsComponent()
        {
            _repositoryMock.Setup(library => library.Contains(It.Is<Type>(type => ReferenceEquals(type, _testComponent.GetType()))))
                .Returns(false);

            _entity.AddComponent(_testComponent);

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Add(typeof(TestComponent), _testComponent), Times.Once);
        }

        [Test]
        public void Negative_AddComponent_DuplicatesComponent_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            Assert.Throws<ComponentAlreadyExistsException>(() => _entity.AddComponent(_testComponent));

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Add(typeof(TestComponent), _testComponent), Times.Never);
        }

        [Test]
        public void Positive_RemoveComponent_RemovesComponent()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            _entity.RemoveComponent<TestComponent>();

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Remove(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Positive_GetComponent_ContainsComponent_ShouldReturnTrue()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            bool contains = _entity.ContainsComponent<TestComponent>();

            Assert.That(contains, Is.True);
            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Positive_GetComponent_DoesNotComponent_ShouldReturnFalse()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(false);

            bool contains = _entity.ContainsComponent<TestComponent>();

            Assert.That(contains, Is.False);
            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Negative_RemoveComponent_MissingComponent_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(false);

            Assert.Throws<ComponentNotFoundException>(() => _entity.RemoveComponent<TestComponent>());

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Remove(typeof(TestComponent)), Times.Never);
        }

        [Test]
        public void Positive_GetComponent_ReturnsComponent()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            _repositoryMock.Setup(library => library.Get(typeof(TestComponent)))
                .Returns(_testComponent);

            IComponent component = _entity.GetComponent<TestComponent>();

            Assert.That(component, Is.EqualTo(_testComponent));

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Get(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Negative_GetComponent_NotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(false);

            Assert.Throws<ComponentNotFoundException>(() => _entity.GetComponent<TestComponent>());

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Get(typeof(TestComponent)), Times.Never);
        }

        [Test]
        public void Positive_TryGetComponent_GetsComponent_ReturnsTrue()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(true);

            _repositoryMock.Setup(library => library.Get(typeof(TestComponent)))
                .Returns(_testComponent);

            bool contains = _entity.TryGetComponent(out TestComponent component);
            Assert.Multiple(() =>
            {
                Assert.That(contains, Is.True);
                Assert.That(component, Is.EqualTo(_testComponent));
            });

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Get(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Negative_TryGetComponent_NotFound_ReturnsFalse()
        {
            _repositoryMock.Setup(library => library.Contains(typeof(TestComponent)))
                .Returns(false);

            bool contains = _entity.TryGetComponent(out TestComponent _);
            Assert.That(contains, Is.False);

            _repositoryMock.Verify(library => library.Contains(typeof(TestComponent)), Times.Once);
            _repositoryMock.Verify(library => library.Get(typeof(TestComponent)), Times.Never);
        }
    }
}