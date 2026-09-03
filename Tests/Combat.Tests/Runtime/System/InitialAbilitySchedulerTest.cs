using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Interface;
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
        private Mock<IAbilityEntityRepository> _abilityRepositoryMock;
        private Mock<IAbilityInitializer> _combatAbilityInitializerMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        private Mock<ITriggerSubscriber> _triggerSubscriberMock;
        
        private CombatantEntity _combatantEntity;
        private AbilityEntity _abilityEntity;
        private AgilityCard _attackerAgility;
        private CombatantCreation _attackerCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _abilityRepositoryMock = new Mock<IAbilityEntityRepository>();
            _combatAbilityInitializerMock = new Mock<IAbilityInitializer>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            _triggerSubscriberMock = new Mock<ITriggerSubscriber>();
            
            _basicAttackScheduler = new InitialAbilityScheduler(_combatantRepositoryMock.Object, _abilityRepositoryMock.Object, _combatAbilityInitializerMock.Object, _abilityEventSchedulerMock.Object, _triggerSubscriberMock.Object);

            _attackerAgility = new AgilityCard { Speed = 10, Initiative = 1 };
            _attackerCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN);
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY, _attackerCreation);
            _abilityEntity = TestAbilityEntityFactory.CreateWithCastTime(1, 1, 10);
            
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

        private void SetupCombatantRepositoryEnumerate(params CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.Enumerate()).Returns(combatantEntities).Verifiable();
        }

        private void SetupAbilityRepositoryContains(byte combatantID)
        {
            _abilityRepositoryMock.Setup(library => library.Contains(combatantID)).Returns(true).Verifiable();
        }

        private void SetupAbilityRepositoryGetAll(byte combatantID, params AbilityEntity[] abilityEntities)
        {
            _abilityRepositoryMock.Setup(library => library.EnumerateAbilities(combatantID)).Returns(abilityEntities).Verifiable();
        }

        private void VerifyAbilityRepositoryContainsCalled(params byte[] combatantIDs)
        {
            foreach (byte combatantID in combatantIDs)
            {
                _abilityRepositoryMock.Verify(library => library.Contains(combatantID), Times.Once);
            }
        }

        private void VerifyInitializeAbilities(CombatantEntity combatantEntity, AbilityEntity[] combatantAbilityEntities)
        {
            _combatAbilityInitializerMock.Verify(library => library.InitializeAbilities(combatantEntity, combatantAbilityEntities), Times.Once);
        }

        private void VerifyScheduleEventCalled(double currentTick, AbilityEntity abilityEntity, Times times)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick, abilityEntity.AbilityID, 0, abilityEntity.InstanceID), times);
        }

        private void VerifySubscribeAbility(params AbilityEntity[] combatantAbilityEntities)
        {
            foreach (AbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            { 
                _triggerSubscriberMock.Verify(library => library.SubscribeAbility(combatantAbilityEntity), Times.Once);
            }
        }

        [Test]
        public void Positive_EnqueueInitial_NoCreatedAbility_NoEnqueue()
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Health = 1 }, _attackerAgility);
            CombatantEntity combatantEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY, combatantCreation);
            SetupCombatantRepositoryEnumerate(combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyAbilityRepositoryContainsCalled(1);
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupCombatantRepositoryEnumerate(_combatantEntity);
            SetupAbilityRepositoryContains(_combatantEntity.InstanceID);
            SetupAbilityRepositoryGetAll(_combatantEntity.InstanceID, _abilityEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyInitializeAbilities(_combatantEntity, [_abilityEntity]);
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative * 0.05, _abilityEntity, Times.Once());
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesMultipleAttackEvent()
        {
            SetupAbilityRepositoryContains(_combatantEntity.InstanceID);
            SetupAbilityRepositoryGetAll(_combatantEntity.InstanceID, _abilityEntity);
            SetupCombatantRepositoryEnumerate(_combatantEntity, 
                TestCombatantEntityFactory.Create(2, TargetingType.FRIENDLY), 
                TestCombatantEntityFactory.Create(3, TargetingType.FRIENDLY), 
                TestCombatantEntityFactory.Create(4, TargetingType.FRIENDLY), 
                TestCombatantEntityFactory.Create(5, TargetingType.FRIENDLY));
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            VerifyInitializeAbilities(_combatantEntity, [_abilityEntity]);
            VerifyAbilityRepositoryContainsCalled(1, 2, 3, 4, 5);
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative * 0.05, _abilityEntity, Times.Once());
        }
        
        [Test]
        public void Positive_EnqueueInitial_ZeroTick()
        {
            SetupAbilityRepositoryContains(_combatantEntity.InstanceID);
            SetupAbilityRepositoryGetAll(_combatantEntity.InstanceID, _abilityEntity);
            SetupCombatantRepositoryEnumerate(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(0d));
            
            VerifyInitializeAbilities(_combatantEntity, [_abilityEntity]);
            VerifyAbilityRepositoryContainsCalled(1);
            VerifyScheduleEventCalled(0d - _attackerAgility.Initiative * 0.05, _abilityEntity, Times.Once());
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));
            
            _combatantRepositoryMock.Verify(library => library.Enumerate(), Times.Once());
        }

        [Test]
        public void Positive_EnqueueInitial_DifferentTriggerType_SubscribesTriggerAbility()
        { 
            _abilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 1,  MaxTriggerValue = 1 });
            
            SetupAbilityRepositoryContains(_combatantEntity.InstanceID);
            SetupAbilityRepositoryGetAll(_combatantEntity.InstanceID, _abilityEntity);
            SetupCombatantRepositoryEnumerate(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.ScheduleRegisteredAbilities(0d));
            
            VerifyAbilityRepositoryContainsCalled(1);
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(0d, _abilityEntity.AbilityID, 0, _abilityEntity.InstanceID), Times.Never);
            VerifyInitializeAbilities(_combatantEntity, [_abilityEntity]);
            VerifySubscribeAbility(_abilityEntity);
        }

        [Test]
        public void Negative_EnqueueInitial_RepositoryReturnsDuplicateCombatants_Throws()
        {
            SetupAbilityRepositoryContains(_combatantEntity.InstanceID);
            SetupAbilityRepositoryGetAll(_combatantEntity.InstanceID, _abilityEntity);
            SetupCombatantRepositoryEnumerate(_combatantEntity, _combatantEntity);
            
            Assert.Throws<ComponentAlreadyExistsException>(() => _basicAttackScheduler.ScheduleRegisteredAbilities(1d));

            _abilityRepositoryMock.Verify(library => library.Contains(1), Times.Exactly(2));
            _combatAbilityInitializerMock.Verify(library => library.InitializeAbilities(_combatantEntity, new [] {_abilityEntity}), Times.Exactly(2));
            VerifyScheduleEventCalled(1d - _attackerAgility.Initiative * 0.05, _abilityEntity, Times.Exactly(1));
        }
    }
}