using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Event.Resolver;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class RetaliationAbilityEffectResolverTest : BaseAbilityEffectResolver
    {
        private RetaliationAbilityEffectResolver _retaliationAbilityEffectResolver;
        private Mock<IEntityDamageSystem> _damageServiceMock;

        private AbilityEntity _retaliationAbility;
        private readonly TargetingPreferenceComponent _targetingPreferenceComponent = new() { CombatantStatType = CombatantStatType.HEALTH, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageServiceMock = new Mock<IEntityDamageSystem>();
            
            _retaliationAbilityEffectResolver = new RetaliationAbilityEffectResolver(CombatantRepositoryMock.Object, TargetFinderMock.Object, CombatantLoggerMock.Object, _damageServiceMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            AbilityStage abilityStage = new()
            {
                AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.HOLY, MaxTargets = 2, Priority = 0, CastTime = 3, Value = 4 },
                TargetingPreferenceComponent = _targetingPreferenceComponent
            };
            
            _retaliationAbility = TestAbilityEntityFactory.Create(1, 3, abilityStage);
            
            _damageServiceMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _damageServiceMock.Verify();
            _damageServiceMock.VerifyNoOtherCalls();
        }

        private static void AddRetaliationComponent(CombatantEntity combatantEntity)
        {
            RetaliationComponent retaliationComponent = new() { Capacity = 3 };
            combatantEntity.AddComponent(retaliationComponent);
            
            retaliationComponent.Enqueue(new CombatantDamageComponent { CombatantID = 2, DamageValue = 3 });
        }

        private void VerifyDamageApplied(CombatantEntity[] targetCombatants, AbilityStage abilityStage, double tick)
        {
            _damageServiceMock.Verify(library => library.ApplyDamage(targetCombatants, 1, abilityStage, tick), Times.Once);
        }

        [Test]
        public void Positive_ResolveEvent_NoRetaliationComponent_NoAction()
        {
            SetupRepositoryGet(InitiatingCombatant);
            
            Assert.DoesNotThrow(() => _retaliationAbilityEffectResolver.ResolveEffect(TICK, _retaliationAbility, GetCombatantAbilityStage(_retaliationAbility, 0)));
        }

        [Test]
        public void Positive_ResolveEvent_ContainsComponent_ReturnsOneDamageComponent()
        {
            SetupRepositoryGet(InitiatingCombatant, TargetCombatant);
            AddRetaliationComponent(InitiatingCombatant);
            
            Assert.DoesNotThrow(() => _retaliationAbilityEffectResolver.ResolveEffect(TICK, _retaliationAbility, GetCombatantAbilityStage(_retaliationAbility, 0)));
            
            VerifyDamageApplied([TargetCombatant], GetCombatantAbilityStage(_retaliationAbility, 0), TICK);
            VerifyCombatantLog(_retaliationAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], GetCombatantAbilityStage(_retaliationAbility, 0));
        }

        [Test]
        public void Positive_ResolveEvent_DealsDamageTillMaxTargets()
        {
            SetupRepositoryGet(InitiatingCombatant, TargetCombatant);
            AddRetaliationComponent(InitiatingCombatant);

            CombatantDamageComponent combatantDamageComponent = new() { CombatantID = InitiatingCombatant.InstanceID, DamageValue = 3 };            
            RetaliationComponent retaliationComponent = InitiatingCombatant.GetComponent<RetaliationComponent>();
            retaliationComponent.Enqueue(combatantDamageComponent);
            retaliationComponent.Enqueue(combatantDamageComponent with { CombatantID = TargetCombatant.InstanceID });
            
            Assert.DoesNotThrow(() => _retaliationAbilityEffectResolver.ResolveEffect(TICK, _retaliationAbility, GetCombatantAbilityStage(_retaliationAbility, 0)));
            
            VerifyDamageApplied([TargetCombatant, InitiatingCombatant], GetCombatantAbilityStage(_retaliationAbility, 0), TICK);
            VerifyCombatantLog(_retaliationAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant, InitiatingCombatant], GetCombatantAbilityStage(_retaliationAbility, 0));
        }

        [Test]
        public void Positive_ResolveEvent_DuplicateID_Filters()
        {
            SetupRepositoryGet(TargetCombatant, InitiatingCombatant);
            AddRetaliationComponent(InitiatingCombatant);

            CombatantDamageComponent combatantDamageComponent = new() { CombatantID = TargetCombatant.InstanceID, DamageValue = 3 };            
            RetaliationComponent retaliationComponent = InitiatingCombatant.GetComponent<RetaliationComponent>();
            retaliationComponent.Enqueue(combatantDamageComponent);
            retaliationComponent.Enqueue(combatantDamageComponent);
            
            Assert.DoesNotThrow(() => _retaliationAbilityEffectResolver.ResolveEffect(TICK, _retaliationAbility, GetCombatantAbilityStage(_retaliationAbility, 0)));
            VerifyDamageApplied([TargetCombatant], GetCombatantAbilityStage(_retaliationAbility, 0), TICK);
            VerifyCombatantLog(_retaliationAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], GetCombatantAbilityStage(_retaliationAbility, 0));
        }
    }
}