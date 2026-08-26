using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantAbilityInitializerTest
    {
        private CombatantAbilityInitializer _combatantAbilityInitializer;

        private CombatantEntity _combatantEntity;
        private CombatantAbilityEntity _directDamageAbility;
        private CombatantAbilityEntity _retaliationAbility;
        
        private readonly CombatantAbilityStage _retaliationAbilityStage = new()
        {
            AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 1, Value = 3, Priority = 0 },
            TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY }
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityInitializer = new CombatantAbilityInitializer();
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _directDamageAbility = TestCombatantAbilityEntityFactory.Create(1, 1);
            _retaliationAbility = TestCombatantAbilityEntityFactory.Create(1, 2, _retaliationAbilityStage);
        }

        private static void AssertContainsRetaliationComponent(CombatantEntity combatantEntity, bool contains)
        {
            Assert.That(combatantEntity.ContainsComponent<RetaliationComponent>(), Is.EqualTo(contains));
        }

        private static void AssertRetaliationComponentCapacity(CombatantEntity combatantEntity, byte capacity)
        {
            RetaliationComponent retaliationComponent = combatantEntity.GetComponent<RetaliationComponent>();
            Assert.That(retaliationComponent.Capacity, Is.EqualTo(capacity));
        }
        
        [Test]
        public void Positive_InitializeAbilities_NoRetaliationStages_DoesNothing()
        {
            Assert.DoesNotThrow(() => _combatantAbilityInitializer.InitializeAbilities(_combatantEntity, [_directDamageAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, false);
        }

        [Test]
        public void Positive_InitializeAbilities_HasRetaliationStage_AddsComponent()
        {
            Assert.DoesNotThrow(() => _combatantAbilityInitializer.InitializeAbilities(_combatantEntity, [_directDamageAbility, _retaliationAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, true);
            AssertRetaliationComponentCapacity(_combatantEntity, _retaliationAbilityStage.AbilityStage.MaxTargets);
        }

        [Test]
        public void Negative_InitializeAbilities_CapacityOverflow_Throws()
        {
            CombatantAbilityStage maxTargetsStage = _retaliationAbilityStage with { AbilityStage = _retaliationAbilityStage.AbilityStage with { MaxTargets = byte.MaxValue }};
            CombatantAbilityEntity maxTargetsEntity = TestCombatantAbilityEntityFactory.Create(3, 3, maxTargetsStage);
            
            Assert.Throws<OverflowException>(() => _combatantAbilityInitializer.InitializeAbilities(_combatantEntity, [maxTargetsEntity, _retaliationAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, false);
        }
    }
}