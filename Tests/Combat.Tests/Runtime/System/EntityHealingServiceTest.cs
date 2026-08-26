using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Service;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityHealingServiceTest
    {
        private EntityHealingService _entityHealingService;

        private CombatantEntity _healingCombatant;
        private CombatantEntity _friendlyTargetCombatant;
        private readonly CombatantAbilityStage _combatantAbilityStage = new()
        {
            AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.COLD, MaxTargets = 1, Value = 3, Priority = 0, CastTime = 0 },
            TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.HEALTH, TargetingPreference = TargetingPreference.LOWEST, TargetingType = TargetingType.FRIENDLY }
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _entityHealingService = new EntityHealingService();
        }

        [SetUp]
        public void Setup()
        { 
            _healingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(combatantID: 2);
            _friendlyTargetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(combatantID: 5);
        }

        private static void ChangeCombatantHealth(CombatantEntity combatantEntity, uint newHealth) => combatantEntity.ReplaceComponent(new HealthComponent { Health = newHealth });

        private static void AssertCombatantHealth(CombatantEntity combatantEntity, uint expectedHealth)
        {
            uint health = combatantEntity.GetComponent<HealthComponent>().Health;
            Assert.That(expectedHealth, Is.EqualTo(health));
        }

        [Test]
        public void Positive_ApplyHealing_HealsEntity()
        {
            ChangeCombatantHealth(_friendlyTargetCombatant, 10);
            
            Assert.DoesNotThrow(() => _entityHealingService.ApplyHealing([_friendlyTargetCombatant], _healingCombatant, _combatantAbilityStage, 0));
            
            AssertCombatantHealth(_friendlyTargetCombatant, 13);
        }

        [Test]
        public void Positive_ApplyHealing_HealsMultipleEntities_CanHealCaster()
        {
            ChangeCombatantHealth(_friendlyTargetCombatant, 10);
            ChangeCombatantHealth(_healingCombatant, 10);
            
            Assert.DoesNotThrow(() => _entityHealingService.ApplyHealing([_friendlyTargetCombatant, _friendlyTargetCombatant, _healingCombatant], _healingCombatant, _combatantAbilityStage, 0));
            
            AssertCombatantHealth(_friendlyTargetCombatant, 16);
            AssertCombatantHealth(_healingCombatant, 13);
        }

        [Test]
        public void Positive_ApplyHealing_HealsMoreThanMax_HealsToValue()
        {
            CombatantAbilityStage beegHeal = _combatantAbilityStage with { AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.COLD, MaxTargets = 1, Value = uint.MaxValue, Priority = 0, CastTime = 0}};
            
            ChangeCombatantHealth(_friendlyTargetCombatant, 10);
            
            Assert.DoesNotThrow(() => _entityHealingService.ApplyHealing([_friendlyTargetCombatant], _healingCombatant, beegHeal, 0));
            
            AssertCombatantHealth(_friendlyTargetCombatant, _friendlyTargetCombatant.GetComponent<BaseHealthComponent>().Health);
        }

        [Test]
        public void Positive_ApplyHealing_EntityOverMaxHealth_DoesNothing()
        {
            uint overMaxHealth = _friendlyTargetCombatant.GetComponent<BaseHealthComponent>().Health + 1;
            ChangeCombatantHealth(_friendlyTargetCombatant, overMaxHealth);
            
            Assert.DoesNotThrow(() => _entityHealingService.ApplyHealing([_friendlyTargetCombatant], _healingCombatant, _combatantAbilityStage, 0));
            
            AssertCombatantHealth(_friendlyTargetCombatant, overMaxHealth);
        }

        [Test]
        public void Positive_ApplyHealing_HealthComponentIsReplaced()
        {
            CombatantAbilityStage zeroHeal = _combatantAbilityStage with { AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.COLD, MaxTargets = 1, Value = 0, Priority = 0, CastTime = 0 }};
            
            HealthComponent healthComponent = _friendlyTargetCombatant.GetComponent<HealthComponent>();
            ChangeCombatantHealth(_friendlyTargetCombatant, 18);
            
            Assert.DoesNotThrow(() => _entityHealingService.ApplyHealing([_friendlyTargetCombatant], _healingCombatant, zeroHeal, 0));
            
            Assert.That(healthComponent, Is.Not.EqualTo(_friendlyTargetCombatant.GetComponent<HealthComponent>()));
            Assert.That(healthComponent.Health, Is.Not.EqualTo(_friendlyTargetCombatant.GetComponent<HealthComponent>().Health));
        }
    }
}