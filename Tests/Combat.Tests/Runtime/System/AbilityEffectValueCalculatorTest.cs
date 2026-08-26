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
    public sealed class AbilityEffectValueCalculatorTest
    {
        private AbilityEffectValueCalculator _abilityEffectValueCalculator;

        private CombatantAbilityEntity _singleStageEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEffectValueCalculator = new AbilityEffectValueCalculator();
        }

        [SetUp]
        public void Setup()
        { 
            _singleStageEntity = TestCombatantAbilityEntityFactory.Create(1, 1);
        }

        private static void VerifyComponentsAdded(CombatantAbilityEntity combatantAbilityEntity)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntity.ContainsComponent<AbilityDamageComponent>(), Is.True);
                Assert.That(combatantAbilityEntity.ContainsComponent<AbilityHealingComponent>(), Is.True);
            }
        }

        private static void VerifyComponentValue(CombatantAbilityEntity combatantAbilityEntity, uint damageValue, uint healingValue)
        {
            AbilityDamageComponent damageComponent = combatantAbilityEntity.GetComponent<AbilityDamageComponent>();
            AbilityHealingComponent healingComponent = combatantAbilityEntity.GetComponent<AbilityHealingComponent>();
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(damageComponent.TotalDamage, Is.EqualTo(damageValue));
                Assert.That(healingComponent.TotalHealing, Is.EqualTo(healingValue));
            }
        }

        [Test]
        public void Positive_Calculate_OneDamageStage_AddsBothComponents()
        {
            Assert.DoesNotThrow(() => _abilityEffectValueCalculator.Calculate(_singleStageEntity));

            VerifyComponentsAdded(_singleStageEntity);
            VerifyComponentValue(_singleStageEntity, 3, 0);
        }

        [Test]
        public void Positive_Calculate_MultipleStages_CalculatesValues()
        {
            TargetingPreferenceComponent targetingPreferenceComponent = new()
            {
                CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY
            };

            AbilityStage abilityStage = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 10, Value = 10, Priority = 0
            };
            
            CombatantAbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStage = abilityStage with { AbilityEffectType = AbilityEffectType.HEALING },
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStage = abilityStage,
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStage = abilityStage,
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStage = abilityStage with { AbilityEffectType = AbilityEffectType.HEALING },
                    TargetingPreferenceComponent = targetingPreferenceComponent
                }
            ];
            
            CombatantAbilityEntity combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, 1, combatantStages);
            
            Assert.DoesNotThrow(() => _abilityEffectValueCalculator.Calculate(combatantAbilityEntity));

            VerifyComponentsAdded(combatantAbilityEntity);
            VerifyComponentValue(combatantAbilityEntity, 20, 20);
        }

        [Test]
        public void Negative_Calculate_UnknownAbilityEffectType_Throws()
        {
            CombatantAbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStage = new AbilityStage
                    {
                        AbilityEffectType = (AbilityEffectType) byte.MaxValue, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 10, Value = 10, Priority = 0
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY
                    }
                }
            ];
            
            CombatantAbilityEntity combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, 1, combatantStages);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => _abilityEffectValueCalculator.Calculate(combatantAbilityEntity));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntity.ContainsComponent<AbilityDamageComponent>(), Is.False);
                Assert.That(combatantAbilityEntity.ContainsComponent<AbilityHealingComponent>(), Is.False);
            }
        }
    }
}