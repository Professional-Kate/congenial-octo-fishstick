using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Tests.TestFactory;
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
        private Mock<IAssetRepository<AbilityType, AbilityEntity>> _abilityEntityRepositoryMock; 
        private Mock<IAbilityEntityFactory> _factoryMock;
        private Mock<IDispatchMany<AbilityCreationResponse>> _responseDispatcherMock;
        private Mock<IAssetRepository<AbilityType, EventType>> _eventRepositoryMock;

        private AbilityCreation _abilityCreation;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEntityRepositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            _factoryMock = new Mock<IAbilityEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityCreationResponse>>();
            _eventRepositoryMock = new Mock<IAssetRepository<AbilityType, EventType>>();
            
            _mediator = new AbilityCreationMediator(_abilityEntityRepositoryMock.Object, _factoryMock.Object, _eventRepositoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new NumberAssertion());
            _abilityCreation = TestAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        {
            _abilityEntityRepositoryMock.Reset();
            _factoryMock.Reset();
            _responseDispatcherMock.Reset();
            _eventRepositoryMock.Reset();
        }

        [TearDown]
        public void TearDown()
        { 
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _abilityEntityRepositoryMock.Verify();
            _abilityEntityRepositoryMock.VerifyNoOtherCalls();
            _factoryMock.Verify();
            _factoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
            _eventRepositoryMock.Verify();
            _eventRepositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyAbilityEntityContains(params AbilityCreation[] abilityCreations)
        {
            foreach (AbilityCreation abilityCreation in abilityCreations)
            {
                _abilityEntityRepositoryMock.Verify(library => library.Contains(abilityCreation.AbilityType), Times.Once);
            }
        }
        
        private void VerifyAbilityEntityAdd(params AbilityCreation[] abilityCreations)
        {
            foreach (AbilityCreation abilityCreation in abilityCreations)
            {
                _abilityEntityRepositoryMock.Verify(library => library.Add(abilityCreation.AbilityType, It.IsAny<AbilityEntity>()), Times.Once);
            }
        }
        
        private void VerifyFactoryCalled(params AbilityCreation[] abilityCreations)
        {
            foreach (AbilityCreation abilityCreation in abilityCreations)
            {
                _factoryMock.Verify(library => library.CreateAbilityEntity(abilityCreation), Times.Once);
            }
        }
        
        private void VerifyEventTypeAdd(AbilityType abilityType, EventType eventType)
        {
            _eventRepositoryMock.Verify(library => library.Add(abilityType, eventType), Times.Once);
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
            VerifyAbilityEntityContains(_abilityCreation);
            VerifyAbilityEntityAdd(_abilityCreation);
            VerifyEventTypeAdd(_abilityCreation.AbilityType, _abilityCreation.EventType);
            VerifyDispatcherCalled(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            AbilityCreation strongAttackCreation = _abilityCreation with { AbilityType = AbilityType.STRONG_ATTACK };
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_abilityCreation, strongAttackCreation]));

            VerifyFactoryCalled(_abilityCreation, strongAttackCreation);
            VerifyAbilityEntityContains(_abilityCreation, strongAttackCreation);
            VerifyAbilityEntityAdd(_abilityCreation, strongAttackCreation);
            VerifyEventTypeAdd(_abilityCreation.AbilityType, _abilityCreation.EventType);
            VerifyEventTypeAdd(strongAttackCreation.AbilityType, strongAttackCreation.EventType);
            VerifyDispatcherCalled(2);
        }

        [Test]
        public void Negative_HandleMessages_DuplicateAbilityType_Throws()
        {
            _abilityEntityRepositoryMock.Setup(library => library.Contains(_abilityCreation.AbilityType)).Returns(true).Verifiable();
            
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