using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Filter.Interface;
using IdelPog.Combat.Core.Logging;
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
        protected AbilityEntity InitiatingAbility { get; private set; }
        protected AbilityStage FirstAbilityStage { get; private set; }
        protected CombatantEntity TargetCombatant { get; private set; }

        [SetUp]
        protected void BaseSetup()
        {
            TargetFinderMock.Reset();
            CombatantRepositoryMock.Reset();
            CombatantLoggerMock.Reset();

            InitiatingCombatant = TestCombatantEntityFactory.Create(combatantID: 1, TargetingType.FRIENDLY);
            InitiatingAbility = TestAbilityEntityFactory.Create(InitiatingCombatant.InstanceID, abilityID: 1);
            FirstAbilityStage = GetCombatantAbilityStage(InitiatingAbility, 0);
                
            TargetCombatant = TestCombatantEntityFactory.Create(combatantID: 2, TargetingType.ENEMY);
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

        protected static AbilityStage GetCombatantAbilityStage(AbilityEntity abilityEntity, int index) => abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[index];
        
        protected void SetupTargetFinder(CombatantEntity target, TargetingPreference targetingPreference, CombatantStatType combatantStatType, byte targetCount, TargetingType targetingType)
        {
            TargetFinderMock.Setup(library => library.SelectPreferredTargets(targetingPreference, combatantStatType, targetingType, It.IsAny<TargetingType>(), targetCount)).Returns([target]).Verifiable();
        }

        protected void SetupRepositoryGet(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                CombatantRepositoryMock.Setup(library => library.Get(combatantEntity.InstanceID)).Returns(combatantEntity).Verifiable();
            }
        }

        protected void VerifyCombatantLog(byte abilityID, double tick, CombatantEntity initiatingEntity, CombatantEntity[] targetCombatants, AbilityStage abilityStage)
        {
            CombatantLoggerMock.Verify(library => library.LogCombatantChange(tick, initiatingEntity, targetCombatants, abilityStage.AbilityStageCards, abilityID));
        }
    }
}