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
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        
        private CombatantEntity _targetEntity;
        private HealthComponent _healthComponent;
        private CombatantAbilityStage _combatantAbilityStage;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystem = new DamageSystem();
            _combatantAbilityStage = new CombatantAbilityStage
            {
                AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, MaxTargets = 1, Value = 10, Priority = 0, CastTime = 0 },
                TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.INITIATIVE, TargetingPreference = TargetingPreference.HIGHEST , TargetingType = TargetingType.ENEMY }
            };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = TestCombatantEntityFactory.CreateCombatantEntity(0);
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
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _combatantAbilityStage);
            
            AssertNewHealth(newHealth,_healthComponent.Health - _combatantAbilityStage.AbilityStage.Value);
            AssertEntityHealth(_targetEntity, newHealth);
        }
        
        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            AbilityStage strongStage = _combatantAbilityStage.AbilityStage with { Value = uint.MaxValue };
            
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _combatantAbilityStage with { AbilityStage = strongStage });
            
            AssertNewHealth(newHealth, 0);
            AssertEntityHealth(_targetEntity, newHealth);
        }

        [Test]
        public void Positive_GetCalculatedDamage_ReturnsCalculatedDamage()
        {
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(_combatantAbilityStage);
            
            Assert.That(calculatedDamage, Is.EqualTo(_combatantAbilityStage.AbilityStage.Value));
        }

        [Test]
        public void Positive_ZeroDamageFromStage_ReturnsZero()
        {
            CombatantAbilityStage weakAbilityStage = new()
            {
                AbilityStage = new AbilityStage
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
            
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(weakAbilityStage);
            Assert.That(calculatedDamage, Is.Zero);

            uint newHealth = _damageSystem.DealDamage(_targetEntity, weakAbilityStage);
            Assert.That(newHealth, Is.EqualTo(_healthComponent.Health));
        }
    }
}