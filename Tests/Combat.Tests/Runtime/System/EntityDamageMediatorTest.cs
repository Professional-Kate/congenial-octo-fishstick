using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityDamageMediatorTest
    {
        private EntityDamageMediator _entityDamageMediator;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantStatsComponent _attackerStatsComponent;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IDamageSystem>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            
            _entityDamageMediator = new EntityDamageMediator(_damageSystemMock.Object, _deathSystemMock.Object, _combatantStoreServiceMock.Object, _combatantLoggerMock.Object);
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(2, AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _attackerStatsComponent = _attackingCombatant.GetComponent<CombatantStatsComponent>();
            
            _damageSystemMock.Reset();
            _deathSystemMock.Reset();
            _combatantLoggerMock.Reset();
            _combatantStoreServiceMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
            _deathSystemMock.Verify();
            _deathSystemMock.VerifyNoOtherCalls();
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
            _combatantLoggerMock.Verify();
            _combatantLoggerMock.VerifyNoOtherCalls();
        }

        private void SetupDamageSystem(CombatantEntity targetCombatant, CombatantStatsComponent attackerStats, uint newHealth, CombatantAbilityEntity combatantAbilityEntity)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, attackerStats.Attack, combatantAbilityEntity)).Returns(newHealth).Verifiable();
        }
        
        private void SetupGetCalculatedDamage(uint attackingCombatantAttack, CombatantAbilityEntity attackingAbility, uint calculatedDamage)
        {
            _damageSystemMock.Setup(library => library.GetCalculatedDamage(attackingCombatantAttack, attackingAbility)).Returns(calculatedDamage).Verifiable();
        }

        private void VerifyStoreRegisterCombatantChange(CombatantEntity combatantEntity)
        {
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantChange(combatantEntity), Times.Once);
        }
        
        private void VerifyLogCombatantChange(CombatantEntity changedEntity, byte attackerID, AbilityType attackerAbility, uint calculatedDamage)
        {
            _combatantLoggerMock.Verify(library => library.LogCombatantChange(changedEntity, attackerID, attackerAbility, calculatedDamage), Times.Once);
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 1, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackerStatsComponent.Attack, _attackingCombatantAbility, 10);
            
            _entityDamageMediator.ApplyDamage(_targetCombatant, _attackingCombatant, _attackingCombatantAbility);

            VerifyLogCombatantChange(_targetCombatant, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10);
            VerifyStoreRegisterCombatantChange(_targetCombatant);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 0, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackerStatsComponent.Attack, _attackingCombatantAbility, 10);
            
            _entityDamageMediator.ApplyDamage(_targetCombatant, _attackingCombatant, _attackingCombatantAbility);
            
            _deathSystemMock.Verify(library => library.KillEntity(_targetCombatant), Times.Once);
            VerifyLogCombatantChange(_targetCombatant, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10);
            VerifyMocks();
        }
    }
}