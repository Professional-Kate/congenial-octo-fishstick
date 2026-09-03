using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityEffectValueCalculatorTest
    {
        private AbilityEffectValueCalculator _abilityEffectValueCalculator;

        private AbilityEntity _singleStageEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEffectValueCalculator = new AbilityEffectValueCalculator();
        }

        [SetUp]
        public void Setup()
        { 
            _singleStageEntity = TestAbilityEntityFactory.Create(1, 1);
        }

        private static void VerifyComponentsAdded(AbilityEntity abilityEntity)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity.ContainsComponent<AbilityDamageComponent>(), Is.True);
                Assert.That(abilityEntity.ContainsComponent<AbilityHealingComponent>(), Is.True);
            }
        }

        private static void VerifyComponentValue(AbilityEntity abilityEntity, uint damageValue, uint healingValue)
        {
            AbilityDamageComponent damageComponent = abilityEntity.GetComponent<AbilityDamageComponent>();
            AbilityHealingComponent healingComponent = abilityEntity.GetComponent<AbilityHealingComponent>();
            
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

            AbilityStageCard abilityStage = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 10, Value = 10, Priority = 0
            };
            
            AbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStageCards = abilityStage with { AbilityEffectType = AbilityEffectType.HEALING },
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStageCards = abilityStage,
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStageCards = abilityStage,
                    TargetingPreferenceComponent = targetingPreferenceComponent
                },
                new()
                {
                    AbilityStageCards = abilityStage with { AbilityEffectType = AbilityEffectType.HEALING },
                    TargetingPreferenceComponent = targetingPreferenceComponent
                }
            ];
            
            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(1, 1, combatantStages);
            
            Assert.DoesNotThrow(() => _abilityEffectValueCalculator.Calculate(abilityEntity));

            VerifyComponentsAdded(abilityEntity);
            VerifyComponentValue(abilityEntity, 20, 20);
        }

        [Test]
        public void Negative_Calculate_UnknownAbilityEffectType_Throws()
        {
            AbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStageCards = new AbilityStageCard
                    {
                        AbilityEffectType = (AbilityEffectType) byte.MaxValue, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 10, Value = 10, Priority = 0
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY
                    }
                }
            ];
            
            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(1, 1, combatantStages);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => _abilityEffectValueCalculator.Calculate(abilityEntity));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity.ContainsComponent<AbilityDamageComponent>(), Is.False);
                Assert.That(abilityEntity.ContainsComponent<AbilityHealingComponent>(), Is.False);
            }
        }
    }
}