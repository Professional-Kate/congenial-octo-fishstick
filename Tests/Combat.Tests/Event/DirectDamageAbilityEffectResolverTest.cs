using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event.Resolver;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class DirectDamageAbilityEffectResolverTest : BaseAbilityEffectResolver
    {
        private DirectDamageAbilityEffectResolver _directDamageAbilityEffectResolver;
        private Mock<IEntityDamageSystem> _damageServiceMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageServiceMock = new Mock<IEntityDamageSystem>();
            
            _directDamageAbilityEffectResolver = new DirectDamageAbilityEffectResolver(CombatantRepositoryMock.Object, TargetFinderMock.Object, CombatantLoggerMock.Object, _damageServiceMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _damageServiceMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _damageServiceMock.Verify();
            _damageServiceMock.VerifyNoOtherCalls();
        }

        private void VerifyDamageApplied(CombatantEntity[] targetCombatants, AbilityStage abilityStage, double tick)
        {
            _damageServiceMock.Verify(library => library.ApplyDamage(targetCombatants, 1, abilityStage, tick), Times.Once);
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage()
        {
            SetupTargetFinder(TargetCombatant, TargetingPreference.HIGHEST, CombatantStatType.HEALTH, 1, TargetingType.ENEMY);
            SetupRepositoryGet(InitiatingCombatant);
            
            Assert.DoesNotThrow(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility, FirstAbilityStage));

            VerifyDamageApplied([TargetCombatant], FirstAbilityStage, TICK);
            VerifyCombatantLog(InitiatingAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], FirstAbilityStage);
        }

        [Test]
        public void Positive_ResolveEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.Create(3, TargetingType.FRIENDLY);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility with { InstanceID = deadEntity.InstanceID }, FirstAbilityStage));
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            CombatantRepositoryMock.Setup(library => library.Get(InitiatingCombatant.InstanceID))
                .Throws(new NotFoundException<byte>(InitiatingCombatant.InstanceID));
            
            Assert.Throws<NotFoundException<byte>>(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingAbility, FirstAbilityStage));
            
            CombatantRepositoryMock.Verify(library => library.Get(InitiatingCombatant.InstanceID), Times.Once);
        }
    }
}