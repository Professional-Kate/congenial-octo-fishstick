using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityDamageMediatorTest
    {
        private EntityDamageMediator _entityDamageMediator;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IDamageSystem>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            
            _entityDamageMediator = new EntityDamageMediator(_damageSystemMock.Object, _deathSystemMock.Object, _combatantLoggerMock.Object);
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(2, AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            
            _damageSystemMock.Reset();
            _deathSystemMock.Reset();
            _combatantLoggerMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
            _deathSystemMock.Verify();
            _deathSystemMock.VerifyNoOtherCalls();
            _combatantLoggerMock.Verify();
            _combatantLoggerMock.VerifyNoOtherCalls();
        }

        private void SetupDamageSystem(CombatantEntity targetCombatant, uint newHealth, CombatantAbilityEntity combatantAbilityEntity)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, combatantAbilityEntity)).Returns(newHealth).Verifiable();
        }
        
        private void SetupGetCalculatedDamage(CombatantAbilityEntity attackingAbility, uint calculatedDamage)
        {
            _damageSystemMock.Setup(library => library.GetCalculatedDamage(attackingAbility)).Returns(calculatedDamage).Verifiable();
        }
        
        private void VerifyLogCombatantChange(CombatantEntity changedEntity, byte attackerID, AbilityType attackerAbility, uint calculatedDamage, double tick)
        {
            _combatantLoggerMock.Verify(library => library.LogCombatantChange(changedEntity, attackerID, attackerAbility, calculatedDamage, tick), Times.Once);
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, 1, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackingCombatantAbility, 10);
            
            _entityDamageMediator.ApplyDamage([_targetCombatant], _attackingCombatant, _attackingCombatantAbility, 1d);

            VerifyLogCombatantChange(_targetCombatant, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10, 1d);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, 0, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackingCombatantAbility, 10);
            
            _entityDamageMediator.ApplyDamage([_targetCombatant], _attackingCombatant, _attackingCombatantAbility, 1d);
            
            _deathSystemMock.Verify(library => library.KillEntity(_targetCombatant), Times.Once);
            VerifyLogCombatantChange(_targetCombatant, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10, 1d);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_ApplyDamage_MultipleTargets_SomeDie()
        {
            SetupDamageSystem(_targetCombatant, 1, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackingCombatantAbility, 10);
            
            CombatantEntity secondTarget = TestCombatantEntityFactory.CreateCombatantEntity(5);
            SetupDamageSystem(secondTarget, 0, _attackingCombatantAbility);
            SetupGetCalculatedDamage(_attackingCombatantAbility, 10);
            
            _entityDamageMediator.ApplyDamage([_targetCombatant, secondTarget], _attackingCombatant, _attackingCombatantAbility, 1d);
            
            _deathSystemMock.Verify(library => library.KillEntity(secondTarget), Times.Once);
            VerifyLogCombatantChange(_targetCombatant, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10, 1d);
            VerifyLogCombatantChange(secondTarget, _attackingCombatant.CombatantID, _attackingCombatantAbility.AbilityType, 10, 1d);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_NoTargets_DoesNothing()
        {
            _entityDamageMediator.ApplyDamage([], _attackingCombatant, _attackingCombatantAbility, 1d);

            VerifyMocks();
        }
    }
}