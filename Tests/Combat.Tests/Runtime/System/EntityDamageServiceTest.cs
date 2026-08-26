using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityDamageServiceTest
    {
        private EntityDamageService _entityDamageService;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ITriggerAbilityHandler<CombatantDamagedData>> _combatantDamagedTriggerMock;
        private Mock<ITriggerAbilityHandler<CombatantDeathData>> _combatantDiedTriggerMock;

        private const double TICK = 1D;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IDamageSystem>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantDamagedTriggerMock =  new Mock<ITriggerAbilityHandler<CombatantDamagedData>>();
            _combatantDiedTriggerMock = new Mock<ITriggerAbilityHandler<CombatantDeathData>>();
            
            _entityDamageService = new EntityDamageService(_damageSystemMock.Object, _combatantDamagedTriggerMock.Object, _deathSystemMock.Object, _combatantDiedTriggerMock.Object);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1, TargetingType.ENEMY);
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(2, 1);
            
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

        private void SetupDamageSystem(CombatantEntity targetCombatant, uint newHealth, CombatantAbilityStage combatantAbilityStage)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, combatantAbilityStage)).Returns(newHealth).Verifiable();
        }

        private void SetupGetCalculatedDamage(CombatantAbilityStage combatantAbilityStage)
        {
            _damageSystemMock.Setup(library => library.GetCalculatedDamage(combatantAbilityStage)).Returns(combatantAbilityStage.AbilityStage.Value).Verifiable();
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
        
        private static CombatantAbilityStage GetFirstAbilityStage(CombatantAbilityEntity combatantAbility) => combatantAbility.GetComponent<AbilityStagesComponent>().AbilityStages[0];

        private static CombatantDamagedData CreateCombatantDamagedData(CombatantEntity combatantEntity, uint damageValue, byte initiatingCombatantID)
        {
            return new CombatantDamagedData
            {
                DamagedCombatantID = combatantEntity.CombatantID,
                DamagedCombatantTargetingType = combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType,
                DamageValue = damageValue,
                InitiatingCombatantID = initiatingCombatantID
            };
        }
        
        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, 1, GetFirstAbilityStage(_attackingCombatantAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingCombatantAbility));
            
            _entityDamageService.ApplyDamage([_targetCombatant], _attackingCombatant.CombatantID, GetFirstAbilityStage(_attackingCombatantAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingCombatantAbility).AbilityStage.Value, _attackingCombatant.CombatantID));
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, 0, GetFirstAbilityStage(_attackingCombatantAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingCombatantAbility));
            
            _entityDamageService.ApplyDamage([_targetCombatant], _attackingCombatant.CombatantID, GetFirstAbilityStage(_attackingCombatantAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingCombatantAbility).AbilityStage.Value, _attackingCombatant.CombatantID));
            VerifyKillEntity(_targetCombatant);
            VerifyDeathTriggerHandle(_targetCombatant.GetComponent<TargetingTypeComponent>().TargetingType, _targetCombatant.CombatantID);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_ApplyDamage_MultipleTargets_SomeDie()
        {
            SetupDamageSystem(_targetCombatant, 1, GetFirstAbilityStage(_attackingCombatantAbility));
            
            CombatantEntity secondTarget = TestCombatantEntityFactory.CreateCombatantEntity(5);
            SetupDamageSystem(secondTarget, 0, GetFirstAbilityStage(_attackingCombatantAbility));
            SetupGetCalculatedDamage(GetFirstAbilityStage(_attackingCombatantAbility));
            
            _entityDamageService.ApplyDamage([_targetCombatant, secondTarget], _attackingCombatant.CombatantID, GetFirstAbilityStage(_attackingCombatantAbility), TICK);

            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(_targetCombatant, GetFirstAbilityStage(_attackingCombatantAbility).AbilityStage.Value, _attackingCombatant.CombatantID));
            VerifyDamagedTriggerHandle(CreateCombatantDamagedData(secondTarget, GetFirstAbilityStage(_attackingCombatantAbility).AbilityStage.Value, _attackingCombatant.CombatantID));
            VerifyKillEntity(secondTarget);
            VerifyDeathTriggerHandle(secondTarget.GetComponent<TargetingTypeComponent>().TargetingType, secondTarget.CombatantID);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_NoTargets_DoesNothing()
        {
            _entityDamageService.ApplyDamage([], _attackingCombatant.CombatantID, GetFirstAbilityStage(_attackingCombatantAbility), TICK);

            VerifyMocks();
        }
    }
}