using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Contracts.Response;
using IdelPog.Combat.Ability.Mediator;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Exceptions;
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
        private Mock<IIncrementalRepository<AbilityDefinition>> _abilityDefinitionRepositoryMock;
        private Mock<IPrioritySorter> _prioritySorterMock;
        private Mock<IDispatchMany<AbilityCreationResponse>> _responseDispatcherMock;

        private AbilityCreation _abilityCreation;
        private readonly AbilityDefinition _abilityDefinition = new()
        {
            AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
            TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
            AbilityStages =
            [
                new AbilityStageCard
                {
                    AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, CastTime = 10, MaxTargets = 5, Priority = 0, Value = 2
                }
            ]
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityDefinitionRepositoryMock = new Mock<IIncrementalRepository<AbilityDefinition>>();
            _prioritySorterMock = new Mock<IPrioritySorter>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityCreationResponse>>();
            
            _abilityCreationMediator = new AbilityCreationMediator(_abilityDefinitionRepositoryMock.Object, _prioritySorterMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new NumberAssertion(), new TriggerAssertion());

            _abilityCreation = TestAbilityCreationFactory.Create(_abilityDefinition);
        }

        [SetUp]
        public void Setup()
        {
            _abilityDefinitionRepositoryMock.Reset();
            _prioritySorterMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _abilityDefinitionRepositoryMock.Verify();
            _abilityDefinitionRepositoryMock.VerifyNoOtherCalls();
            _prioritySorterMock.Verify();
            _prioritySorterMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void VerifyAbilityDefinitionAdd(AbilityDefinition abilityDefinition)
        {
            _abilityDefinitionRepositoryMock.Verify(library => library.Add(It.Is<AbilityDefinition>(definition => definition.AbilityCard == abilityDefinition.AbilityCard && definition.TriggerCard == abilityDefinition.TriggerCard)), Times.Once);
        }

        private void SetupSort(AbilityStageCard[] abilityStageCards)
        {
            _prioritySorterMock.Setup(library => library.Sort(abilityStageCards, It.IsAny<Func<AbilityStageCard, byte>>())).Returns([..abilityStageCards]).Verifiable();
        }
        
        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<AbilityCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            SetupSort(_abilityCreation.AbilityStageCards);
            
            Assert.DoesNotThrow(() => _abilityCreationMediator.HandleMessages([_abilityCreation]));

            VerifyAbilityDefinitionAdd(_abilityDefinition);
            VerifyDispatcherCalled(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            SetupSort(_abilityCreation.AbilityStageCards);
            
            AbilityCard abilityCard = _abilityCreation.AbilityCard with { AbilitySlots = 2 };
            AbilityCreation strongAttackCreation = _abilityCreation with { AbilityCard = abilityCard };
            
            Assert.DoesNotThrow(() => _abilityCreationMediator.HandleMessages([_abilityCreation, strongAttackCreation]));

            VerifyAbilityDefinitionAdd(_abilityDefinition);
            VerifyAbilityDefinitionAdd(_abilityDefinition with { AbilityCard = abilityCard });
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
        public void Negative_HandleMessage_AbilityReadyTrigger_NotConfiguredCorrectly_Throws()
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