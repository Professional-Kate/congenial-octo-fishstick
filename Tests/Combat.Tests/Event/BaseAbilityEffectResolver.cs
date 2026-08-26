using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    public abstract class BaseAbilityEffectResolver
    {
        protected const double TICK = 10D;
        
        protected readonly Mock<ICombatantTargetFinder> TargetFinderMock = new();
        protected readonly Mock<ICombatantRepository> CombatantRepositoryMock = new();
        protected readonly Mock<ICombatantLogger> CombatantLoggerMock = new();

        protected CombatantEntity InitiatingCombatant { get; private set; }
        protected CombatantAbilityEntity InitiatingCombatantAbility { get; private set; }
        protected CombatantAbilityStage FirstAbilityStage { get; private set; }
        protected CombatantEntity TargetCombatant { get; private set; }

        [SetUp]
        protected void BaseSetup()
        {
            TargetFinderMock.Reset();
            CombatantRepositoryMock.Reset();
            CombatantLoggerMock.Reset();

            InitiatingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(combatantID: 1);
            InitiatingCombatantAbility = TestCombatantAbilityEntityFactory.Create(InitiatingCombatant.CombatantID, abilityID: 1);
            FirstAbilityStage = GetCombatantAbilityStage(InitiatingCombatantAbility, 0);
                
            TargetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(combatantID: 2, TargetingType.ENEMY);
        }

        [TearDown]
        protected void BaseTearDown()
        {
            TargetFinderMock.Verify();
            TargetFinderMock.VerifyNoOtherCalls();
            CombatantRepositoryMock.Verify();
            CombatantRepositoryMock.VerifyNoOtherCalls();
            CombatantLoggerMock.Verify();
            CombatantLoggerMock.VerifyNoOtherCalls();
        }

        protected static CombatantAbilityStage GetCombatantAbilityStage(CombatantAbilityEntity combatantAbilityEntity, int index) => combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[index];
        
        protected void SetupTargetFinder(CombatantEntity target, TargetingPreference targetingPreference, CombatantStatType combatantStatType, byte targetCount, TargetingType targetingType)
        {
            TargetFinderMock.Setup(library => library.SelectPreferredTargets(targetingPreference, combatantStatType, targetingType, It.IsAny<TargetingType>(), targetCount)).Returns([target]).Verifiable();
        }

        protected void SetupRepositoryGet(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                CombatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
            }
        }

        protected void VerifyCombatantLog(byte abilityID, double tick, CombatantEntity initiatingEntity, CombatantEntity[] targetCombatants, CombatantAbilityStage combatantAbilityStage)
        {
            CombatantLoggerMock.Verify(library => library.LogCombatantChange(tick, initiatingEntity, targetCombatants, combatantAbilityStage.AbilityStage, abilityID));
        }
    }
}