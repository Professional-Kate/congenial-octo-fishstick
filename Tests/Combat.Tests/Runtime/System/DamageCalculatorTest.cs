using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageCalculatorTest
    {
        private DamageCalculator _damageCalculator;
        
        private CombatantEntity _targetEntity;
        private HealthComponent _healthComponent;
        private AbilityStage _abilityStage;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageCalculator = new DamageCalculator();
            _abilityStage = new AbilityStage
            {
                AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, MaxTargets = 1, Value = 10, Priority = 0, CastTime = 0 },
                TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.INITIATIVE, TargetingPreference = TargetingPreference.HIGHEST , TargetingType = TargetingType.ENEMY }
            };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = TestCombatantEntityFactory.Create(0, TargetingType.FRIENDLY);
            _healthComponent = _targetEntity.GetComponent<HealthComponent>();
        }

        private static void AssertNewHealth(uint newHealth, uint expectedHealth)
        { 
            Assert.That(newHealth, Is.EqualTo(expectedHealth));
        }

        private static void AssertEntityHealth(CombatantEntity combatantEntity, uint expectedHealth)
        {
            Assert.That(combatantEntity.GetComponent<HealthComponent>().Health, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity()
        {
            uint newHealth = _damageCalculator.DealDamage(_targetEntity, _abilityStage);
            
            AssertNewHealth(newHealth,_healthComponent.Health - _abilityStage.AbilityStageCards.Value);
            AssertEntityHealth(_targetEntity, newHealth);
        }
        
        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            AbilityStageCard strongStage = _abilityStage.AbilityStageCards with { Value = uint.MaxValue };
            
            uint newHealth = _damageCalculator.DealDamage(_targetEntity, _abilityStage with { AbilityStageCards = strongStage });
            
            AssertNewHealth(newHealth, 0);
            AssertEntityHealth(_targetEntity, newHealth);
        }

        [Test]
        public void Positive_GetCalculatedDamage_ReturnsCalculatedDamage()
        {
            uint calculatedDamage = _damageCalculator.GetCalculatedDamage(_abilityStage);
            
            Assert.That(calculatedDamage, Is.EqualTo(_abilityStage.AbilityStageCards.Value));
        }

        [Test]
        public void Positive_ZeroDamageFromStage_ReturnsZero()
        {
            AbilityStage weakAbilityStage = new()
            {
                AbilityStageCards = new AbilityStageCard
                {
                    AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, 
                    AffinityType = AffinityType.SLASH, 
                    MaxTargets = 1, 
                    Value = 0,
                    Priority = 0,
                    CastTime = 0
                },
                TargetingPreferenceComponent = new TargetingPreferenceComponent
                {
                    CombatantStatType = CombatantStatType.HEALTH,
                    TargetingPreference = TargetingPreference.HIGHEST,
                    TargetingType = TargetingType.ENEMY
                }
            };
            
            uint calculatedDamage = _damageCalculator.GetCalculatedDamage(weakAbilityStage);
            Assert.That(calculatedDamage, Is.Zero);

            uint newHealth = _damageCalculator.DealDamage(_targetEntity, weakAbilityStage);
            Assert.That(newHealth, Is.EqualTo(_healthComponent.Health));
        }
    }
}