using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantAbilityEntityFactoryTest
    {
        private CombatantAbilityEntityFactory _combatantAbilityEntityFactory;
        private Mock<IIncrementalRepository<AbilityEntity>> _repositoryMock;
        private Mock<IPrioritySorter> _prioritySorterMock;
        private Mock<IAbilityEffectValueCalculator> _abilityEffectValueCalculatorMock;

        private CombatantAbilityEquip _combatantAbilityEquip;
        private AbilityEntity _abilityEntity;
        private CombatantAbilityCard _combatantAbilityCard;
        private TriggerComponent _triggerComponent;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IIncrementalRepository<AbilityEntity>>();
            _prioritySorterMock = new Mock<IPrioritySorter>();
            _abilityEffectValueCalculatorMock = new Mock<IAbilityEffectValueCalculator>();
            
            _combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(_repositoryMock.Object, _prioritySorterMock.Object, _abilityEffectValueCalculatorMock.Object, new PriorityAssertion());

            _combatantAbilityCard = new CombatantAbilityCard { AbilityID = 0, StrategyCards = [new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_combatantAbilityCard] };
            _triggerComponent = new TriggerComponent { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.ABILITY_READY, MinTriggerValue = 0, MaxTriggerValue = 0 };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _prioritySorterMock.Reset();
            _abilityEffectValueCalculatorMock.Reset();
            
            _abilityEntity = new AbilityEntity(new CooldownComponent { Cooldown = 10 }, _triggerComponent)
            {
                AbilitySlots = 1,
                AbilityStages = [new AbilityStage { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, MaxTargets = 1, Value = 2, Priority = 0, CastTime = 0 }]
            };
        }

        private void VerifyRepository()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            _prioritySorterMock.Verify();
            _prioritySorterMock.VerifyNoOtherCalls();
            _abilityEffectValueCalculatorMock.Verify();
            _abilityEffectValueCalculatorMock.VerifyNoOtherCalls();
        }

        private void SetupPrioritySorter(IReadOnlyList<StrategyCard> strategyCards, params StrategyCard[] sortedCards)
        {
            _prioritySorterMock.Setup(library => library.Sort(strategyCards, It.IsAny<Func<StrategyCard, byte>>())).Returns(sortedCards).Verifiable();
        }

        private void SetupRepositoryGet(AbilityEntity abilityEntity, byte abilityID)
        {
            _repositoryMock.Setup(library => library.Get(abilityID)).Returns(abilityEntity).Verifiable();
        }

        private void VerifyCalculate(params AbilityEntity[] abilityEntities)
        {
            foreach (AbilityEntity abilityEntity in abilityEntities)
            { 
                _abilityEffectValueCalculatorMock.Verify(library => library.Calculate(It.Is<CombatantAbilityEntity>(entity => entity.AbilitySlots == abilityEntity.AbilitySlots)), Times.Once);
            }
        }
        
        private static void AssertCollectionCount(int count, IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        {
            Assert.That(combatantAbilityEntities, Has.Count.EqualTo(count));
        }

        private static void AssertCombatantAbility(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity, byte combatantID, byte abilityID)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntity.AbilityID, Is.EqualTo(abilityID));
                Assert.That(combatantAbilityEntity.CombatantID, Is.EqualTo(combatantID));
                Assert.That(combatantAbilityEntity.GetComponent<CooldownComponent>(), Is.EqualTo(abilityEntity.GetComponent<CooldownComponent>()));
            }
        }

        [Test]
        public void Positive_Create_CreatesNewEntity_AddsExpectedComponents()
        {
            SetupRepositoryGet(_abilityEntity, 0);
            SetupPrioritySorter(_combatantAbilityCard.StrategyCards, _combatantAbilityCard.StrategyCards);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(_combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, _combatantAbilityEquip.CombatantID, 0);
            VerifyCalculate(_abilityEntity);
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Create_DuplicateEquip_ReturnsTwoEntities()
        {
            CombatantAbilityEquip doubleEquip = _combatantAbilityEquip with { AbilityCards = [_combatantAbilityCard, _combatantAbilityCard] };
            
            SetupRepositoryGet(_abilityEntity, 0);
            SetupPrioritySorter(_combatantAbilityCard.StrategyCards, _combatantAbilityCard.StrategyCards);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(doubleEquip);
            
            AssertCollectionCount(2,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, doubleEquip.CombatantID, 0);
            AssertCombatantAbility(combatantAbilityEntities[1], _abilityEntity, doubleEquip.CombatantID, 0);
            _abilityEffectValueCalculatorMock.Verify(library => library.Calculate(It.Is<CombatantAbilityEntity>(entity => entity.AbilitySlots == _abilityEntity.AbilitySlots)), Times.Exactly(2));
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Create_NoAbilityCards_ReturnsEmptyCollection()
        {
            CombatantAbilityEquip noCards = _combatantAbilityEquip with { AbilityCards = [] };
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(noCards);
            
            AssertCollectionCount(0,  combatantAbilityEntities);
            VerifyRepository();
        }

        [Test]
        public void Positive_Create_CreatesEntity_WithCastTime()
        {
            SetupRepositoryGet(_abilityEntity, 0);
            SetupPrioritySorter(_combatantAbilityCard.StrategyCards, _combatantAbilityCard.StrategyCards);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(_combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, _combatantAbilityEquip.CombatantID, 0);
            VerifyCalculate(_abilityEntity);
            VerifyRepository();
        }

        [Test]
        public void Positive_Create_MultipleAbilityStages()
        {
            AbilityStage[] abilityStages = 
            [
                new() { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                new() { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 }
            ];

            StrategyCard highHealthCard = new()
            {
                CombatantStatType = CombatantStatType.HEALTH,
                TargetingType = TargetingType.ENEMY,
                TargetingPreference = TargetingPreference.HIGHEST,
                Priority = 0
            };

            CombatantAbilityCard combatantAbilityCard = new() { AbilityID = 0, StrategyCards = [highHealthCard, highHealthCard with { Priority = 1 }]};
            CombatantAbilityEquip combatantAbilityEquip = new()
            {
                CombatantID = 1,
                AbilityCards = [combatantAbilityCard]
            };
            
            AbilityEntity multipleStagesEntity = TestAbilityEntityFactory.Create(abilityStages);
            
            SetupRepositoryGet(multipleStagesEntity, 0);
            SetupPrioritySorter(combatantAbilityCard.StrategyCards, combatantAbilityCard.StrategyCards);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], multipleStagesEntity, combatantAbilityEquip.CombatantID, 0);
            _prioritySorterMock.Verify(library => library.Sort(combatantAbilityCard.StrategyCards, It.IsAny<Func<StrategyCard, byte>>()), Times.Once);
            VerifyCalculate(_abilityEntity);
            VerifyRepository();
        }

        [Test]
        public void Negative_Create_AbilityNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Get(_combatantAbilityCard.AbilityID))
                .Throws(new NotFoundException<byte>(0)).Verifiable();
            
            Assert.Throws<NotFoundException<byte>>(() => _combatantAbilityEntityFactory.Create(_combatantAbilityEquip));
            
            VerifyRepository();
        }

        [Test]
        public void Negative_Create_PriorityMismatch_Throws()
        {
            StrategyCard strategyCard = new()
            {
                TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 213
            };

            CombatantAbilityCard combatantAbilityCard = new() { AbilityID = 1, StrategyCards = [strategyCard] };
            CombatantAbilityEquip combatantAbilityEquip = new()
            {
                CombatantID = 1,
                AbilityCards = [combatantAbilityCard]
            };

            SetupRepositoryGet(_abilityEntity, 1);
            SetupPrioritySorter(combatantAbilityCard.StrategyCards, combatantAbilityCard.StrategyCards);
            
            PriorityMismatchException exception = Assert.Throws<PriorityMismatchException>(() => _combatantAbilityEntityFactory.Create(combatantAbilityEquip));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exception.AbilityStagePriority, Is.Zero);
                Assert.That(exception.StrategyCardPriority, Is.EqualTo(strategyCard.Priority));
            }
            
            _prioritySorterMock.Verify(library => library.Sort(combatantAbilityCard.StrategyCards, It.IsAny<Func<StrategyCard, byte>>()), Times.Once);
        }
    }
}