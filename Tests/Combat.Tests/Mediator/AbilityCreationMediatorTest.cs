using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class AbilityCreationMediatorTest
    {
        private AbilityCreationMediator _mediator;
        private Mock<IAssetRepository<AbilityType, AbilityEntity>> _repositoryMock; 
        private Mock<IAbilityEntityFactory> _factoryMock;
        private Mock<IDispatchMany<AbilityCreationResponse>> _responseDispatcherMock;

        private AbilityCreation _abilityCreation;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            _factoryMock = new Mock<IAbilityEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityCreationResponse>>();
            
            _mediator = new AbilityCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new NumberAssertion());
            _abilityCreation = TestAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        { 
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            _factoryMock.Verify();
            _factoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void AssertRepositoryContains(AbilityCreation abilityCreation)
        {
            _repositoryMock.Verify(library => library.Contains(abilityCreation.AbilityType), Times.Once);
        }
        
        private void AssertRepositoryAdd(AbilityCreation abilityCreation)
        {
            _repositoryMock.Verify(library => library.Add(abilityCreation.AbilityType, It.IsAny<AbilityEntity>()), Times.Once);
        }

        private void VerifyFactoryCalled(AbilityCreation abilityCreation)
        {
            _factoryMock.Verify(library => library.CreateAbilityEntity(abilityCreation), Times.Once);
        }

        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<AbilityCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_abilityCreation]));

            VerifyFactoryCalled(_abilityCreation);
            AssertRepositoryContains(_abilityCreation);
            AssertRepositoryAdd(_abilityCreation);
            VerifyDispatcherCalled(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            AbilityCreation strongAttackCreation = _abilityCreation with { AbilityType = AbilityType.STRONG_ATTACK };
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_abilityCreation, strongAttackCreation]));

            VerifyFactoryCalled(_abilityCreation);
            VerifyFactoryCalled(strongAttackCreation);
            AssertRepositoryContains(_abilityCreation);
            AssertRepositoryContains(strongAttackCreation);
            AssertRepositoryAdd(_abilityCreation);
            AssertRepositoryAdd(strongAttackCreation);
            VerifyDispatcherCalled(2);
        }

        [Test]
        public void Negative_HandleMessages_DuplicateAbilityType_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_abilityCreation.AbilityType)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _mediator.HandleMessages([_abilityCreation]));
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessages_ZeroSpeed_Throws()
        { 
            Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([_abilityCreation with { Cooldown = 0 }]));
        }
    }
}