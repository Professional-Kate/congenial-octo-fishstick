using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityDamageSystemTest
    {
        private EntityDamageSystem _entityDamageSystem;
        private Mock<IDamageCalculator> _damageSystemMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ITriggerAbilityHandler<CombatantDamagedData>> _combatantDamagedTriggerMock;
        private Mock<ITriggerAbilityHandler<CombatantDeathData>> _combatantDiedTriggerMock;

        private const double TICK = 1D;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private AbilityEntity _attackingAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IDamageCalculator>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantDamagedTriggerMock =  new Mock<ITriggerAbilityHandler<CombatantDamagedData>>();
            _combatantDiedTriggerMock = new Mock<ITriggerAbilityHandler<CombatantDeathData>>();
            
            _entityDamageSystem = new EntityDamageSystem(_damageSystemMock.Object, _combatantDamagedTriggerMock.Object, _deathSystemMock.Object, _combatantDiedTriggerMock.Object);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = TestCombatantEntityFactory.Create(1, TargetingType.ENEMY);
            _attackingCombatant = TestCombatantEntityFactory.Create(2, TargetingType.FRIENDLY);
            _attackingAbility = TestAbilityEntityFactory.Create(2, 1);
            
            _damageSystemMock.Reset();
            _deathSystemMock.Reset();
            _combatantDamagedTriggerMock.Reset();
            _combatantDiedTriggerMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
            _deathSystemMock.Verify();
            _deathSystemMock.VerifyNoOtherCalls();
            _combatantDamagedTriggerMock.Verify();
            _combatantDamagedTriggerMock.VerifyNoOtherCalls();
            _combatantDiedTriggerMock.Verify();
            _combatantDiedTriggerMock.VerifyNoOtherCalls();
        }

        private void SetupDamageSystem(CombatantEntity targetCombatant, uint newHealth, AbilityStage abilityStage)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, abilityStage)).Returns(newHealth).Verifiable();
        }

        private void SetupGetCalculatedDamage(AbilityStage abilityStage)
        {
            _damageSystemMock.Setup(library => library.GetCalculatedDamage(abilityStage)).Returns(abilityStage.AbilityStageCards.Value).Verifiable();
        }

        private void VerifyKillEntity(CombatantEntity combatantEntity)
        { 
            _deathSystemMock.Verify(library => library.KillEntity(combatantEntity), Times.Once);
        }

        private void VerifyDamagedTriggerHandle(CombatantDamagedData combatantDamagedData)
        {
            _combatantDamagedTriggerMock.Verify(library => library.Handle(TICK, combatantDamagedData), Times.Once);
        }
        
        private void VerifyDeathTriggerHandle(TargetingType combatantTargetingType, byte deadCombatantID)
        {
            _combatantDiedTriggerMock.Verify(library => library.Handle(TICK, new CombatantDeathData { CombatantTargetingType = combatantTargetingType, DeadCombatantID = deadCombatantID }), Times.Once);
        }
        
        private static AbilityStage GetFirstAbilityStage(AbilityEntity ability) => ability.GetComponent<AbilityStagesComponent>().AbilityStages[0];

        private static CombatantDamagedData CreateCombatantDamagedData(CombatantEntity combatantEntity, uint damageValue, byte initiatingCombatantID)
        {
            return new CombatantDamagedData
            {
                DamagedCombatantID = combatantEntity.InstanceID,
                DamagedCombatantTargetingType = combatantEntity.TargetingType,
                DamageValue = damageValue,
                InitiatingCombatantID = initiatingCombatantID
            };
        }
        
        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, 1, GetFirstAbilityStage(_attackingAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingAbility));
            
            _entityDamageSystem.ApplyDamage([_targetCombatant], _attackingCombatant.InstanceID, GetFirstAbilityStage(_attackingAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingAbility).AbilityStageCards.Value, _attackingCombatant.InstanceID));
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, 0, GetFirstAbilityStage(_attackingAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingAbility));
            
            _entityDamageSystem.ApplyDamage([_targetCombatant], _attackingCombatant.InstanceID, GetFirstAbilityStage(_attackingAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingAbility).AbilityStageCards.Value, _attackingCombatant.InstanceID));
            VerifyKillEntity(_targetCombatant);
            VerifyDeathTriggerHandle(_targetCombatant.TargetingType, _targetCombatant.InstanceID);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_ApplyDamage_MultipleTargets_SomeDie()
        {
            SetupDamageSystem(_targetCombatant, 1, GetFirstAbilityStage(_attackingAbility));
            
            CombatantEntity secondTarget = TestCombatantEntityFactory.Create(5, TargetingType.FRIENDLY);
            SetupDamageSystem(secondTarget, 0, GetFirstAbilityStage(_attackingAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingAbility));
            
            _entityDamageSystem.ApplyDamage([_targetCombatant, secondTarget], _attackingCombatant.InstanceID, GetFirstAbilityStage(_attackingAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingAbility).AbilityStageCards.Value, _attackingCombatant.InstanceID));
            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(secondTarget, GetFirstAbilityStage(_attackingAbility).AbilityStageCards.Value, _attackingCombatant.InstanceID));
            VerifyKillEntity(secondTarget);
            VerifyDeathTriggerHandle(secondTarget.TargetingType, secondTarget.InstanceID);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_NoTargets_DoesNothing()
        {
            _entityDamageSystem.ApplyDamage([], _attackingCombatant.InstanceID, GetFirstAbilityStage(_attackingAbility), TICK);

            VerifyMocks();
        }
    }
}