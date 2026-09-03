using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityInitializerTest
    {
        private AbilityInitializer _abilityInitializer;

        private CombatantEntity _combatantEntity;
        private AbilityEntity _directDamageAbility;
        private AbilityEntity _retaliationAbility;
        
        private readonly AbilityStage _retaliationAbilityStage = new()
        {
            AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 1, Value = 3, Priority = 0 },
            TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY }
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityInitializer = new AbilityInitializer();
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);
            _directDamageAbility = TestAbilityEntityFactory.Create(1, 1);
            _retaliationAbility = TestAbilityEntityFactory.Create(1, 2, _retaliationAbilityStage);
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
            Assert.DoesNotThrow(() => _abilityInitializer.InitializeAbilities(_combatantEntity, [_directDamageAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, false);
        }

        [Test]
        public void Positive_InitializeAbilities_HasRetaliationStage_AddsComponent()
        {
            Assert.DoesNotThrow(() => _abilityInitializer.InitializeAbilities(_combatantEntity, [_directDamageAbility, _retaliationAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, true);
            AssertRetaliationComponentCapacity(_combatantEntity, _retaliationAbilityStage.AbilityStageCards.MaxTargets);
        }

        [Test]
        public void Negative_InitializeAbilities_CapacityOverflow_Throws()
        {
            AbilityStage maxTargetsStage = _retaliationAbilityStage with { AbilityStageCards = _retaliationAbilityStage.AbilityStageCards with { MaxTargets = byte.MaxValue }};
            AbilityEntity maxTargetsEntity = TestAbilityEntityFactory.Create(3, 3, maxTargetsStage);
            
            Assert.Throws<OverflowException>(() => _abilityInitializer.InitializeAbilities(_combatantEntity, [maxTargetsEntity, _retaliationAbility]));
            
            AssertContainsRetaliationComponent(_combatantEntity, false);
        }
    }
}