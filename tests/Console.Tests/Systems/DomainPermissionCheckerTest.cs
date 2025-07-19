using Console.Runtime.ECS;
using Console.Runtime.Systems;
using Console.Types;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace Console.Tests.Systems
{
    [TestFixture]
    public class DomainPermissionCheckerTest
    {
        private IDomainPermissionChecker _domainPermissionChecker;
        private Mock<IEntity> _entityMock;
        private ComponentStore<CommandDomainComponent>  _componentStore;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _entityMock = new Mock<IEntity>();
            _domainPermissionChecker = new DomainPermissionChecker(_entityMock.Object, new AssertComponentFound(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _entityMock.Reset();
        }

        private void SetupComponentStore(CommandDomain allowedDomain)
        {
            CommandDomainComponent[] commandDomainComponents = [new() { AllowedCommandDomain = allowedDomain }];
            _componentStore = new ComponentStore<CommandDomainComponent>(commandDomainComponents, new ThrowHandler());
        }

        [Test]
        public void Positive_IsAllowed_DomainAdded_ReturnsTrue()
        {
            SetupComponentStore(CommandDomain.CURRENCY);

            _entityMock
                .Setup(library => library.TryGetComponent(out _componentStore))
                .Returns(true);

            bool isAllowed = _domainPermissionChecker.IsAllowed(CommandDomain.CURRENCY);
            
            Assert.That(isAllowed, Is.True);
            _entityMock.Verify(library => library.TryGetComponent(out _componentStore), Times.Once);
        }
        
        [Test]
        public void Positive_IsAllowed_DomainNotAdded_ReturnsFalse()
        {
            SetupComponentStore(CommandDomain.SKILL);
            
            _entityMock
                .Setup(library => library.TryGetComponent(out _componentStore))
                .Returns(true);

            bool isAllowed = _domainPermissionChecker.IsAllowed(CommandDomain.CURRENCY);
            
            Assert.That(isAllowed, Is.False);
            _entityMock.Verify(library => library.TryGetComponent(out _componentStore), Times.Once);
        }

        [Test]
        public void Negative_IsAllowed_NoComponentStoreFound_Throws()
        {
            _entityMock.Setup(library => library.ContainsComponent<ComponentStore<CommandDomainComponent>>())
                .Returns(false);
            
            Assert.Throws<ComponentNotFoundException>(() => _domainPermissionChecker.IsAllowed(CommandDomain.CURRENCY));
        }
    }
}