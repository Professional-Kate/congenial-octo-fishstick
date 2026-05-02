using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using Moq;

namespace IdelPog.ECS.Tests.Entity
{
    [TestFixture]
    public sealed class EntityTests
    {
        private ECS.Entity.Entity _entity { get; set; }
        private Mock<IDictionary<Type, IComponent>> _dictionaryMock { get; set; }
        private TestComponent _testComponent { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testComponent = new TestComponent();
            _dictionaryMock = new Mock<IDictionary<Type, IComponent>>();
            _entity = new TestEntity(_dictionaryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dictionaryMock.Reset();
        }

        [Test]
        public void Positive_AddRequiredComponents_AddsComponents()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            _dictionaryMock.Setup(library => library[typeof(TestComponent)])
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
            _dictionaryMock.Setup(library => library.ContainsKey(It.Is<Type>(type => ReferenceEquals(type, _testComponent.GetType()))))
                .Returns(false);

            _entity.AddComponent(_testComponent);

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library.Add(typeof(TestComponent), _testComponent), Times.Once);
        }

        [Test]
        public void Negative_AddComponent_DuplicatesComponent_Throws()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            Assert.Throws<ComponentAlreadyExistsException>(() => _entity.AddComponent(_testComponent));

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library.Add(typeof(TestComponent), _testComponent), Times.Never);
        }

        [Test]
        public void Positive_RemoveComponent_RemovesComponent()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            _entity.RemoveComponent<TestComponent>();

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library.Remove(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Positive_GetComponent_ContainsComponent_ShouldReturnTrue()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            bool contains = _entity.ContainsComponent<TestComponent>();

            Assert.That(contains, Is.True);
            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Positive_GetComponent_DoesNotComponent_ShouldReturnFalse()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(false);

            bool contains = _entity.ContainsComponent<TestComponent>();

            Assert.That(contains, Is.False);
            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
        }

        [Test]
        public void Negative_RemoveComponent_MissingComponent_Throws()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(false);

            Assert.Throws<ComponentNotFoundException>(() => _entity.RemoveComponent<TestComponent>());

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library.Remove(typeof(TestComponent)), Times.Never);
        }

        [Test]
        public void Positive_GetComponent_ReturnsComponent()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            _dictionaryMock.Setup(library => library[typeof(TestComponent)])
                .Returns(_testComponent);

            IComponent component = _entity.GetComponent<TestComponent>();

            Assert.That(component, Is.EqualTo(_testComponent));

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library[typeof(TestComponent)], Times.Once);
        }

        [Test]
        public void Negative_GetComponent_NotFound_Throws()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(false);

            Assert.Throws<ComponentNotFoundException>(() => _entity.GetComponent<TestComponent>());

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library[typeof(TestComponent)], Times.Never);
        }

        [Test]
        public void Positive_TryGetComponent_GetsComponent_ReturnsTrue()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(true);

            _dictionaryMock.Setup(library => library[typeof(TestComponent)])
                .Returns(_testComponent);

            bool contains = _entity.TryGetComponent(out TestComponent component);
            Assert.Multiple(() =>
            {
                Assert.That(contains, Is.True);
                Assert.That(component, Is.EqualTo(_testComponent));
            });

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library[typeof(TestComponent)], Times.Once);
        }

        [Test]
        public void Negative_TryGetComponent_NotFound_ReturnsFalse()
        {
            _dictionaryMock.Setup(library => library.ContainsKey(typeof(TestComponent)))
                .Returns(false);

            bool contains = _entity.TryGetComponent(out TestComponent _);
            Assert.That(contains, Is.False);

            _dictionaryMock.Verify(library => library.ContainsKey(typeof(TestComponent)), Times.Once);
            _dictionaryMock.Verify(library => library[typeof(TestComponent)], Times.Never);
        }
    }
}