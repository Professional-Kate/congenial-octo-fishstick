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
    public sealed class DirectDamageAbilityEffectResolverTest : BaseAbilityEffectResolver
    {
        private DirectDamageAbilityEffectResolver _directDamageAbilityEffectResolver;
        private Mock<IEntityDamageService> _damageServiceMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageServiceMock = new Mock<IEntityDamageService>();
            
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

        private void VerifyDamageApplied(CombatantEntity[] targetCombatants, CombatantAbilityStage combatantAbilityStage, double tick)
        {
            _damageServiceMock.Verify(library => library.ApplyDamage(targetCombatants, 1, combatantAbilityStage, tick), Times.Once);
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage()
        {
            SetupTargetFinder(TargetCombatant, TargetingPreference.HIGHEST, CombatantStatType.HEALTH, 1, TargetingType.ENEMY);
            SetupRepositoryGet(InitiatingCombatant);
            
            Assert.DoesNotThrow(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingCombatantAbility, FirstAbilityStage));

            VerifyDamageApplied([TargetCombatant], FirstAbilityStage, TICK);
            VerifyCombatantLog(InitiatingCombatantAbility.AbilityID, TICK, InitiatingCombatant, [TargetCombatant], FirstAbilityStage);
        }

        [Test]
        public void Positive_ResolveEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.CreateCombatantEntity(3);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingCombatantAbility with { CombatantID = 3 }, FirstAbilityStage));
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            CombatantRepositoryMock.Setup(library => library.Get(InitiatingCombatant.CombatantID))
                .Throws(new NotFoundException<byte>(InitiatingCombatant.CombatantID));
            
            Assert.Throws<NotFoundException<byte>>(() => _directDamageAbilityEffectResolver.ResolveEffect(TICK, InitiatingCombatantAbility, FirstAbilityStage));
            
            CombatantRepositoryMock.Verify(library => library.Get(InitiatingCombatant.CombatantID), Times.Once);
        }
    }
}