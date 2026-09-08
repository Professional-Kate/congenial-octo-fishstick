using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class AbilityEntityFactoryTest
    {
        private AbilityEntityFactory _abilityEntityFactory;
        private Mock<IIncrementalRepository<AbilityDefinition>> _repositoryMock;
        private Mock<IAbilityStatsFactory> _abilityStatsCalculatorMock;

        private AbilityEquip _abilityEquip;
        private AbilityDefinition _abilityDefinition;
        private EquippedAbility _equippedAbility;
        private EquippedAbilityDefinition _equippedAbilityDefinition;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IIncrementalRepository<AbilityDefinition>>();
            _abilityStatsCalculatorMock = new Mock<IAbilityStatsFactory>();
            
            _abilityEntityFactory = new AbilityEntityFactory(_repositoryMock.Object, _abilityStatsCalculatorMock.Object);

            _equippedAbility = new EquippedAbility { AbilityID = 0, StrategyCards = [new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, StatType = StatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            _abilityEquip = new AbilityEquip { CombatantID = 1, EquippedAbilities = [_equippedAbility] };
            _abilityDefinition = new AbilityDefinition
            {
                AbilityCard = new AbilityCard { Cooldown = 4, AbilitySlots = 2 },
                TriggerCard = new TriggerCard { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.ABILITY_READY, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStages = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 1, MaxTargets = 1, Priority = 0, Value = 1 }]
            };

            _equippedAbilityDefinition = new EquippedAbilityDefinition
            {
                CombatantID = 1,
                EquippedAbilities = [_equippedAbility]
            };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _abilityStatsCalculatorMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            _abilityStatsCalculatorMock.Verify();
            _abilityStatsCalculatorMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(AbilityDefinition abilityDefinition, byte abilityID)
        {
            _repositoryMock.Setup(library => library.Get(abilityID)).Returns(abilityDefinition).Verifiable();
        }

        private void SetupCreateStats(params AbilityDefinition[] abilityEntities)
        {
            foreach (AbilityDefinition abilityDefinition in abilityEntities)
            {
                StatComponent[] statComponents =
                [
                    new() { StatType = StatType.COOLDOWN, Stat = abilityDefinition.AbilityCard.Cooldown },
                    new() { StatType = StatType.ABILITY_SLOTS, Stat = abilityDefinition.AbilityCard.AbilitySlots }
                ];
                
                _abilityStatsCalculatorMock.Setup(library => library.CreateStats(It.IsAny<AbilityStagesComponent>(), abilityDefinition.AbilityCard)).Returns(statComponents).Verifiable();
            }
        }
        
        private static void AssertCollectionCount(int count, AbilityEntity[] combatantAbilityEntities)
        {
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(count));
        }

        private static void AssertCombatantAbility(AbilityEntity abilityEntity, AbilityDefinition abilityDefinition, byte combatantID, byte abilityID)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity.AbilityID, Is.EqualTo(abilityID));
                Assert.That(abilityEntity.InstanceID, Is.EqualTo(combatantID));
                Assert.That(abilityEntity.GetStat(StatType.COOLDOWN), Is.EqualTo(abilityDefinition.AbilityCard.Cooldown));
                Assert.That(abilityEntity.GetStat(StatType.ABILITY_SLOTS), Is.EqualTo(abilityDefinition.AbilityCard.AbilitySlots));
            }
        }

        [Test]
        public void Positive_Create_CreatesNewEntity_AddsExpectedComponents()
        {
            SetupRepositoryGet(_abilityDefinition, 0);
            SetupCreateStats(_abilityDefinition);
            
            AbilityEntity[] combatantAbilityEntities = _abilityEntityFactory.Create(_equippedAbilityDefinition, _equippedAbilityDefinition.CombatantID);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityDefinition, _abilityEquip.CombatantID, 0);
        }
        
        [Test]
        public void Positive_Create_DuplicateEquip_ReturnsTwoEntities()
        {
            SetupRepositoryGet(_abilityDefinition, 0);

            EquippedAbilityDefinition doubleEquipDefinition = _equippedAbilityDefinition with
            {
                EquippedAbilities = [_equippedAbility, _equippedAbility]
            };
            SetupCreateStats(_abilityDefinition);
            
            AbilityEntity[] combatantAbilityEntities = _abilityEntityFactory.Create(doubleEquipDefinition, _equippedAbilityDefinition.CombatantID);
            
            AssertCollectionCount(2,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityDefinition, doubleEquipDefinition.CombatantID, 0);
            AssertCombatantAbility(combatantAbilityEntities[1], _abilityDefinition, doubleEquipDefinition.CombatantID, 0);
        }
        
        [Test]
        public void Positive_Create_NoAbilityCards_ReturnsEmptyCollection()
        {
            EquippedAbilityDefinition noCards = _equippedAbilityDefinition with
            {
                EquippedAbilities = []
            };
            
            AbilityEntity[] combatantAbilityEntities = _abilityEntityFactory.Create(noCards, _equippedAbilityDefinition.CombatantID);
            
            AssertCollectionCount(0,  combatantAbilityEntities);
        }

        [Test]
        public void Positive_Create_CreatesEntity_WithCastTime()
        {
            SetupRepositoryGet(_abilityDefinition, 0);
            SetupCreateStats(_abilityDefinition);
            
            AbilityEntity[] combatantAbilityEntities = _abilityEntityFactory.Create(_equippedAbilityDefinition, _equippedAbilityDefinition.CombatantID);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityDefinition, _abilityEquip.CombatantID, 0);
        }

        [Test]
        public void Positive_Create_MultipleAbilityStages()
        {
            StrategyCard highHealthCard = new()
            {
                StatType = StatType.HEALTH,
                TargetingType = TargetingType.ENEMY,
                TargetingPreference = TargetingPreference.HIGHEST,
                Priority = 0
            };

            SetupRepositoryGet(_abilityDefinition, 0);
            SetupCreateStats(_abilityDefinition);
            
            EquippedAbilityDefinition equippedAbilityDefinition = _equippedAbilityDefinition with
            {
                EquippedAbilities = 
                [
                    new EquippedAbility { AbilityID = 0, StrategyCards = [highHealthCard, highHealthCard with { Priority = 1 }] }
                ]
            };
            
            AbilityEntity[] combatantAbilityEntities = _abilityEntityFactory.Create(equippedAbilityDefinition, _equippedAbilityDefinition.CombatantID);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityDefinition, equippedAbilityDefinition.CombatantID, 0);
        }

        [Test]
        public void Negative_Create_AbilityNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Get(_equippedAbility.AbilityID))
                .Throws(new NotFoundException<byte>(0)).Verifiable();
            
            Assert.Throws<NotFoundException<byte>>(() => _abilityEntityFactory.Create(_equippedAbilityDefinition, _equippedAbilityDefinition.CombatantID));
        }
    }
}