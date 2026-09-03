using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Resolver;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class HealingAbilityEffectResolverTest : BaseAbilityEffectResolver
    {
        private HealingAbilityEffectResolver _healingAbilityEffectResolver;
        private Mock<IEntityHealingService> _entityHealingServiceMock;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _entityHealingServiceMock = new Mock<IEntityHealingService>();
            
            _healingAbilityEffectResolver = new HealingAbilityEffectResolver(CombatantRepositoryMock.Object, TargetFinderMock.Object, CombatantLoggerMock.Object, _entityHealingServiceMock.Object);
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
        
        private void VerifyHealingApplied(CombatantEntity[] targetCombatants, CombatantEntity attackingCombatant, AbilityStage abilityStage, double tick)
        {
            _entityHealingServiceMock.Verify(library => library.ApplyHealing(targetCombatants, attackingCombatant, abilityStage, tick), Times.Once);
        }

        [Test]
        public void Positive_HandleEvent_HealsEntity()
        {
            SetupTargetFinder(TargetCombatant, TargetingPreference.HIGHEST, CombatantStatType.HEALTH, 1, TargetingType.ENEMY);
            SetupRepositoryGet(InitiatingCombatant);
            
            Assert.DoesNotThrow(() => _healingAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility, FirstAbilityStage));
            
            VerifyHealingApplied([TargetCombatant], InitiatingCombatant, FirstAbilityStage, TICK);
            VerifyCombatantLog(InitiatingAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], FirstAbilityStage);
        }
        
        [Test]
        public void Positive_HandleEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.Create(25, TargetingType.FRIENDLY);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _healingAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility with { InstanceID = deadEntity.InstanceID }, FirstAbilityStage));
        }
        
        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            CombatantRepositoryMock.Setup(library => library.Get(InitiatingCombatant.InstanceID))
                .Throws(new NotFoundException<byte>(InitiatingCombatant.InstanceID));
            
            Assert.Throws<NotFoundException<byte>>(() => _healingAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility, FirstAbilityStage));
            
            CombatantRepositoryMock.Verify(library => library.Get(InitiatingCombatant.InstanceID), Times.Once);
        }
    }
}