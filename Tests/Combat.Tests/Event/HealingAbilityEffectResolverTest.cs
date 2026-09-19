using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Resolver;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class HealingAbilityEffectResolverTest : BaseAbilityEffectResolver
    {
        private HealingAbilityEffectResolver _healingAbilityEffectResolver;
        private Mock<IEntityHealingSystem> _entityHealingServiceMock;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _entityHealingServiceMock = new Mock<IEntityHealingSystem>();
            
            _healingAbilityEffectResolver = new HealingAbilityEffectResolver(TargetFinderMock.Object, CombatantLoggerMock.Object, _entityHealingServiceMock.Object);
        }
        
        [SetUp]
        public void Setup()
        {
            _entityHealingServiceMock.Reset();
        }

        [TearDown]
        public void TearDown()
        { 
            _entityHealingServiceMock.Verify();
            _entityHealingServiceMock.VerifyNoOtherCalls();
        }
        
        private void VerifyHealingApplied(CombatantEntity[] targetCombatants, AbilityStage abilityStage)
        {
            _entityHealingServiceMock.Verify(library => library.ApplyHealing(targetCombatants, abilityStage), Times.Once);
        }

        [Test]
        public void Positive_HandleEvent_HealsEntity()
        {
            SetupTargetFinder(TargetCombatant, TargetingPreference.HIGHEST, StatType.HEALTH, 1, TargetingType.ENEMY);
            
            Assert.DoesNotThrow(() => _healingAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility, FirstAbilityStage, InitiatingCombatant));
            
            VerifyHealingApplied([TargetCombatant], FirstAbilityStage);
            VerifyCombatantLog(InitiatingAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], FirstAbilityStage);
        }
        
        [Test]
        public void Positive_HandleEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.Create(25, TargetingType.FRIENDLY);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            Assert.DoesNotThrow(() => _healingAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility with { InstanceID = deadEntity.InstanceID }, FirstAbilityStage, deadEntity));
        }
    }
}