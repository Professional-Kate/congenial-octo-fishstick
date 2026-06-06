using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class InitialAbilitySchedulerTest
    {
        private InitialAbilityScheduler _basicAttackScheduler;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantAbilityEntityRepository> _abilityRepositoryMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        
        private CombatantEntity _combatantEntity;
        private CombatantAbilityEntity _combatantAbilityEntity;
        private AgilityCard _attackerAgility;
        private CombatantCreation _attackerCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _abilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            
            _basicAttackScheduler = new InitialAbilityScheduler(_combatantRepositoryMock.Object, _abilityRepositoryMock.Object, _abilityEventSchedulerMock.Object, new NumberAssertion());

            _attackerAgility = new AgilityCard { Speed = 10, Initiative = 1 };
            _attackerCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN);
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _attackerCreation);
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.CreateWithBaseComponents(1, AbilityType.SLASH);
        }

        [SetUp]
        public void Setup()
        {
            _combatantRepositoryMock.Reset();
            _abilityRepositoryMock.Reset();
            _abilityEventSchedulerMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _abilityRepositoryMock.Verify();
            _abilityRepositoryMock.VerifyNoOtherCalls();
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantRepositoryGetAll(params CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.GetAll()).Returns(combatantEntities).Verifiable();
        }

        private void SetupAbilityRepositoryContains(byte combatantID)
        {
            _abilityRepositoryMock.Setup(library => library.Contains(combatantID)).Returns(true).Verifiable();
        }

        private void SetupAbilityRepositoryGetAll(byte combatantID, params CombatantAbilityEntity[] abilityEntities)
        {
            _abilityRepositoryMock.Setup(library => library.GetAll(combatantID)).Returns(abilityEntities).Verifiable();
        }

        private void VerifyAbilityRepositoryContainsCalled(byte combatantID, Times times)
        {
            _abilityRepositoryMock.Verify(library => library.Contains(combatantID), times);
        }

        private void VerifyScheduleEventCalled(double currentTick, CombatantAbilityEntity combatantAbilityEntity, Times times)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick, combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType), times);
        }

        [Test]
        public void Positive_EnqueueInitial_NoCreatedAbility_NoEnqueue()
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Health = 1 }, _attackerCreation.Information, _attackerAgility);
            CombatantEntity combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, combatantCreation);
            SetupCombatantRepositoryGetAll(combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyAbilityRepositoryContainsCalled(1, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupCombatantRepositoryGetAll(_combatantEntity);
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesMultipleAttackEvent()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyAbilityRepositoryContainsCalled(1, Times.Exactly(5));
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Exactly(5));
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueInitial_ZeroTick()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(0d));
            
            VerifyAbilityRepositoryContainsCalled(1, Times.Once());
            VerifyScheduleEventCalled(0d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            _combatantRepositoryMock.Verify(library => library.GetAll(), Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_EnqueueInitial_ZeroSpeed_Throws()
        {
            CombatantEntity zeroInitiativeEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, true, _attackerAgility with { Initiative = 0 });
            SetupCombatantRepositoryGetAll(zeroInitiativeEntity);
            
            SetupAbilityRepositoryContains(zeroInitiativeEntity.CombatantID);
            SetupAbilityRepositoryGetAll(zeroInitiativeEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(zeroInitiativeEntity);
            
            Assert.Throws<NumberZeroException>(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            VerifyAbilityRepositoryContainsCalled(zeroInitiativeEntity.CombatantID, Times.Once());
            VerifyMocks();
        }
    }
}