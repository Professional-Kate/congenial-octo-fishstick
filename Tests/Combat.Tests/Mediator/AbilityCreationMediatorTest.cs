using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class AbilityCreationMediatorTest
    {
        private AbilityCreationMediator _abilityCreationMediator;
        private Mock<IIncrementalRepository<AbilityEntity>> _abilityEntityRepositoryMock; 
        private Mock<IAbilityEntityFactory> _abilityEntityFactoryMock;
        private Mock<IDispatchMany<AbilityCreationResponse>> _responseDispatcherMock;

        private readonly AbilityCreation _abilityCreation = TestAbilityCreationFactory.Create();
        private readonly AbilityEntity _abilityEntity = TestAbilityEntityFactory.Create();
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEntityRepositoryMock = new Mock<IIncrementalRepository<AbilityEntity>>();
            _abilityEntityFactoryMock = new Mock<IAbilityEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityCreationResponse>>();
            
            _abilityCreationMediator = new AbilityCreationMediator(_abilityEntityRepositoryMock.Object, _abilityEntityFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new NumberAssertion(), new TriggerAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _abilityEntityRepositoryMock.Reset();
            _abilityEntityFactoryMock.Reset();
            _responseDispatcherMock.Reset();
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
            _abilityEntityFactoryMock.Verify();
            _abilityEntityFactoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void SetupAbilityEntityFactory(AbilityCreation abilityCreation, AbilityEntity abilityEntity)
        {
            _abilityEntityFactoryMock.Setup(library => library.CreateAbilityEntity(abilityCreation)).Returns(abilityEntity).Verifiable();
        }
        
        private void VerifyAbilityEntityAdd(AbilityEntity abilityEntity)
        {
            _abilityEntityRepositoryMock.Verify(library => library.Add(abilityEntity), Times.Once);
        }
        
        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<AbilityCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            SetupAbilityEntityFactory(_abilityCreation, _abilityEntity);
            
            Assert.DoesNotThrow(() => _abilityCreationMediator.HandleMessages([_abilityCreation]));

            VerifyAbilityEntityAdd(_abilityEntity);
            VerifyDispatcherCalled(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            AbilityCard abilityCard = _abilityCreation.AbilityCard with { AbilitySlots = 2 };
            AbilityCreation strongAttackCreation = _abilityCreation with { AbilityCard = abilityCard };
            
            SetupAbilityEntityFactory(_abilityCreation, _abilityEntity);
            SetupAbilityEntityFactory(strongAttackCreation, _abilityEntity with { AbilitySlots = 2 });
            
            Assert.DoesNotThrow(() => _abilityCreationMediator.HandleMessages([_abilityCreation, strongAttackCreation]));

            VerifyAbilityEntityAdd(_abilityEntity);
            VerifyAbilityEntityAdd(_abilityEntity with { AbilitySlots = 2 });
            VerifyDispatcherCalled(2);
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _abilityCreationMediator.HandleMessages([]));
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _abilityCreationMediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessages_ZeroCooldown_Throws()
        { 
            AbilityCard abilityCard = _abilityCreation.AbilityCard with { Cooldown = 0 };
            Assert.Throws<NumberZeroException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { AbilityCard = abilityCard }]));
        }

        [Test]
        public void Negative_HandleMessages_ZeroMaxTargets_Throws()
        {
            AbilityStageCard zeroTargetsStage = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE,
                AffinityType = AffinityType.COLD,
                MaxTargets = 0,
                Value = 1,
                Priority = 0,
                CastTime = 0
            };
            
            Assert.Throws<NumberZeroException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { AbilityStageCards = [zeroTargetsStage]}]));
        }

        [Test]
        public void Negative_HandleMessages_BadAbilityStageCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { AbilityStageCards = []}]));
            Assert.Throws<ArgumentNullException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { AbilityStageCards = null!}]));
        }

        [Test]
        public void Negative_HandleMessage_AbilityReadyTrigger_NotConfigured_Throws()
        {
            TriggerCard goodTriggerCard = new() { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 };
            TriggerCard notSelfCard = goodTriggerCard with { TargetingType = TargetingType.ENEMY };
            TriggerCard notZeroMinTrigger = goodTriggerCard with { MinTriggerValue = 10 };
            TriggerCard notZeroMaxTrigger = goodTriggerCard with { MaxTriggerValue = 20 };
            
            Assert.Throws<AbilityReadyException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { TriggerCard = notSelfCard }]));
            Assert.Throws<AbilityReadyException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { TriggerCard = notZeroMinTrigger }]));
            Assert.Throws<AbilityReadyException>(() => _abilityCreationMediator.HandleMessages([_abilityCreation with { TriggerCard = notZeroMaxTrigger }]));
        }
    }
}