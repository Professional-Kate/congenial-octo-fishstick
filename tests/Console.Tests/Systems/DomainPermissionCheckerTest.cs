using IdelPog.Console.Runtime.ECS;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;
using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Assertion;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;
using IdelPog.ECS.Exceptions;
using Moq;

namespace IdelPog.Console.Tests.Systems
{
    [TestFixture]
    public class DomainPermissionCheckerTest
    {
        private IDomainPermissionChecker _domainPermissionChecker;
        private Mock<IEntity> _entityMock;
        private ComponentStore<DomainComponent> _componentStore;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _entityMock = new Mock<IEntity>();
            _domainPermissionChecker = new DomainPermissionChecker(_entityMock.Object, new ComponentAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _entityMock.Reset();
        }

        private void SetupComponentStore(Domain allowedDomain)
        {
            DomainComponent[] commandDomainComponents = [new() { AllowedDomain = allowedDomain }];
            _componentStore = new ComponentStore<DomainComponent>(commandDomainComponents, new ThrowHandler());
        }

        [Test]
        public void Positive_IsAllowed_DomainAdded_ReturnsTrue()
        {
            SetupComponentStore(Domain.CURRENCY);

            _entityMock
                .Setup(library => library.TryGetComponent(out _componentStore))
                .Returns(true);

            bool isAllowed = _domainPermissionChecker.IsAllowed(Domain.CURRENCY);

            Assert.That(isAllowed, Is.True);
            _entityMock.Verify(library => library.TryGetComponent(out _componentStore), Times.Once);
        }

        [Test]
        public void Positive_IsAllowed_DomainNotAdded_ReturnsFalse()
        {
            SetupComponentStore(Domain.SKILL);

            _entityMock
                .Setup(library => library.TryGetComponent(out _componentStore))
                .Returns(true);

            bool isAllowed = _domainPermissionChecker.IsAllowed(Domain.CURRENCY);

            Assert.That(isAllowed, Is.False);
            _entityMock.Verify(library => library.TryGetComponent(out _componentStore), Times.Once);
        }

        [Test]
        public void Negative_IsAllowed_NoComponentStoreFound_Throws()
        {
            _entityMock.Setup(library => library.ContainsComponent<ComponentStore<DomainComponent>>())
                .Returns(false);

            Assert.Throws<ComponentNotFoundException>(() => _domainPermissionChecker.IsAllowed(Domain.CURRENCY));
        }
    }
}