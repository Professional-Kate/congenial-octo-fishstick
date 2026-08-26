using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.ECS.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class InitialAbilitySchedulerTest
    {
        private InitialAbilityScheduler _basicAttackScheduler;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantAbilityEntityRepository> _abilityRepositoryMock;
        private Mock<ICombatantAbilityInitializer> _combatAbilityInitializerMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        private Mock<ITriggerSubscriber> _triggerSubscriberMock;
        
        private CombatantEntity _combatantEntity;
        private CombatantAbilityEntity _combatantAbilityEntity;
        private AgilityCard _attackerAgility;
        private CombatantCreation _attackerCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _abilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatAbilityInitializerMock = new Mock<ICombatantAbilityInitializer>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            _triggerSubscriberMock = new Mock<ITriggerSubscriber>();
            
            _basicAttackScheduler = new InitialAbilityScheduler(_combatantRepositoryMock.Object, _abilityRepositoryMock.Object, _combatAbilityInitializerMock.Object, _abilityEventSchedulerMock.Object, _triggerSubscriberMock.Object);

            _attackerAgility = new AgilityCard { Speed = 10, Initiative = 1 };
            _attackerCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN);
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, TargetingType.FRIENDLY, _attackerCreation);
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.CreateWithCastTime(1, 1, 10);
            
            _combatantRepositoryMock.Reset();
            _abilityRepositoryMock.Reset();
            _combatAbilityInitializerMock.Reset();
            _abilityEventSchedulerMock.Reset();
            _triggerSubscriberMock.Reset();
        }
        
        [TearDown]
        public void TearDown()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _abilityRepositoryMock.Verify();
            _abilityRepositoryMock.VerifyNoOtherCalls();
            _combatAbilityInitializerMock.Verify();
            _combatAbilityInitializerMock.VerifyNoOtherCalls();
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
            _triggerSubscriberMock.Verify();
            _triggerSubscriberMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantRepositoryGetAll(params CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.GetAllParticipating()).Returns(combatantEntities).Verifiable();
        }

        private void SetupAbilityRepositoryContains(byte combatantID)
        {
            _abilityRepositoryMock.Setup(library => library.Contains(combatantID)).Returns(true).Verifiable();
        }

        private void SetupAbilityRepositoryGetAll(byte combatantID, params CombatantAbilityEntity[] abilityEntities)
        {
            _abilityRepositoryMock.Setup(library => library.GetAll(combatantID)).Returns(abilityEntities).Verifiable();
        }

        private void VerifyAbilityRepositoryContainsCalled(params byte[] combatantIDs)
        {
            foreach (byte combatantID in combatantIDs)
            {
                _abilityRepositoryMock.Verify(library => library.Contains(combatantID), Times.Once);
            }
        }

        private void VerifyInitializeAbilities(CombatantEntity combatantEntity, CombatantAbilityEntity[] combatantAbilityEntities)
        {
            _combatAbilityInitializerMock.Verify(library => library.InitializeAbilities(combatantEntity, combatantAbilityEntities), Times.Once);
        }

        private void VerifyScheduleEventCalled(double currentTick, CombatantAbilityEntity combatantAbilityEntity, Times times)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick, combatantAbilityEntity.AbilityID, 0, combatantAbilityEntity.CombatantID), times);
        }

        private void VerifySubscribeAbility(params CombatantAbilityEntity[] combatantAbilityEntities)
        {
            foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            { 
                _triggerSubscriberMock.Verify(library => library.SubscribeAbility(combatantAbilityEntity), Times.Once);
            }
        }

        [Test]
        public void Positive_EnqueueInitial_NoCreatedAbility_NoEnqueue()
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Health = 1 }, _attackerAgility);
            CombatantEntity combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, TargetingType.FRIENDLY, combatantCreation);
            SetupCombatantRepositoryGetAll(combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyAbilityRepositoryContainsCalled(1);
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupCombatantRepositoryGetAll(_combatantEntity);
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyInitializeAbilities(_combatantEntity, [_combatantAbilityEntity]);
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Once());
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesMultipleAttackEvent()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity, TestCombatantEntityFactory.CreateCombatantEntity(2), TestCombatantEntityFactory.CreateCombatantEntity(3), TestCombatantEntityFactory.CreateCombatantEntity(4), TestCombatantEntityFactory.CreateCombatantEntity(5));
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyInitializeAbilities(_combatantEntity, [_combatantAbilityEntity]);
            VerifyAbilityRepositoryContainsCalled(1, 2, 3, 4, 5);
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Once());
        }
        
        [Test]
        public void Positive_EnqueueInitial_ZeroTick()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(0d));
            
            VerifyInitializeAbilities(_combatantEntity, [_combatantAbilityEntity]);
            VerifyAbilityRepositoryContainsCalled(1);
            VerifyScheduleEventCalled(0d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Once());
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));
            
            _combatantRepositoryMock.Verify(library => library.GetAllParticipating(), Times.Once());
        }

        [Test]
        public void Positive_EnqueueInitial_DifferentTriggerType_SubscribesTriggerAbility()
        { 
            _combatantAbilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 1,  MaxTriggerValue = 1 });
            
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(0d));
            
            VerifyAbilityRepositoryContainsCalled(1);
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(0d, _combatantAbilityEntity.AbilityID, 0, _combatantAbilityEntity.CombatantID), Times.Never);
            VerifyInitializeAbilities(_combatantEntity, [_combatantAbilityEntity]);
            VerifySubscribeAbility(_combatantAbilityEntity);
        }

        [Test]
        public void Negative_EnqueueInitial_RepositoryReturnsDuplicateCombatants_Throws()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity, _combatantEntity);
            
            Assert.Throws<ComponentAlreadyExistsException>(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            _abilityRepositoryMock.Verify(library => library.Contains(1), Times.Exactly(2));
            _combatAbilityInitializerMock.Verify(library => library.InitializeAbilities(_combatantEntity, new [] {_combatantAbilityEntity}), Times.Exactly(2));
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative, _combatantAbilityEntity, Times.Exactly(1));
        }
    }
}