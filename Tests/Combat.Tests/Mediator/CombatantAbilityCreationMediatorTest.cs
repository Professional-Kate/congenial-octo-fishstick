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
    public sealed class CombatantAbilityCreationMediatorTest
    {
        private CombatantAbilityCreationMediator _mediator;
        private Mock<IAssetRepository<AbilityType, AbilityEntity>> _repositoryMock; 
        private Mock<IAbilityEntityFactory> _factoryMock;
        private Mock<IDispatchMany<CombatantAbilityCreationResponse>> _responseDispatcherMock;

        private CombatantAbilityCreation _combatantAbilityCreation;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            _factoryMock = new Mock<IAbilityEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantAbilityCreationResponse>>();
            
            _mediator = new CombatantAbilityCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new NumberAssertion());
            _combatantAbilityCreation = TestCombatantAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
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

        private void AssertRepositoryContains(CombatantAbilityCreation combatantAbilityCreation)
        {
            _repositoryMock.Verify(library => library.Contains(combatantAbilityCreation.AbilityType), Times.Once);
        }
        
        private void AssertRepositoryAdd(CombatantAbilityCreation combatantAbilityCreation)
        {
            _repositoryMock.Verify(library => library.Add(combatantAbilityCreation.AbilityType, It.IsAny<AbilityEntity>()), Times.Once);
        }

        private void VerifyFactoryCalled(CombatantAbilityCreation combatantAbilityCreation)
        {
            _factoryMock.Verify(library => library.CreateAbilityEntity(combatantAbilityCreation), Times.Once);
        }

        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantAbilityCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantAbilityCreation]));

            VerifyFactoryCalled(_combatantAbilityCreation);
            AssertRepositoryContains(_combatantAbilityCreation);
            AssertRepositoryAdd(_combatantAbilityCreation);
            VerifyDispatcherCalled(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            // TODO: update when we add another AbilityType :)
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantAbilityCreation, _combatantAbilityCreation with { AbilityType = (AbilityType) 2 }]));

            VerifyFactoryCalled(_combatantAbilityCreation);
            VerifyFactoryCalled(_combatantAbilityCreation with { AbilityType = (AbilityType) 2 });
            AssertRepositoryContains(_combatantAbilityCreation);
            AssertRepositoryContains(_combatantAbilityCreation with { AbilityType = (AbilityType) 2 });
            AssertRepositoryAdd(_combatantAbilityCreation);
            AssertRepositoryAdd(_combatantAbilityCreation with { AbilityType = (AbilityType) 2 });
            VerifyDispatcherCalled(2);
        }

        [Test]
        public void Negative_HandleMessages_DuplicateAbilityType_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_combatantAbilityCreation.AbilityType)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _mediator.HandleMessages([_combatantAbilityCreation]));
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
            Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([_combatantAbilityCreation with { Speed = 0 }]));
        }
    }
}