using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilitySchedulerTest
    {
        private AbilityScheduler _basicAttackScheduler;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantAbilityEntityRepository> _abilityRepositoryMock;
        private Mock<ICombatQueue> _combatQueueMock;
        
        private CombatantEntity _combatantEntity;
        private CombatantAbilityEntity _combatantAbilityEntity;
        private StatCard _attackerStats;
        private CombatantCreation _attackerCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _abilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatQueueMock = new Mock<ICombatQueue>();
            
            _basicAttackScheduler = new AbilityScheduler(_combatantRepositoryMock.Object, _abilityRepositoryMock.Object, _combatQueueMock.Object, new NumberAssertion(), new FoundAssertion());

            _attackerStats = new StatCard { Health = 100, Attack = 10, Speed = 10 };
            _attackerCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, _attackerStats);
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _attackerCreation);
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.CreateWithBaseComponents(1, AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        {
            _combatantRepositoryMock.Reset();
            _abilityRepositoryMock.Reset();
            _combatQueueMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _abilityRepositoryMock.Verify();
            _abilityRepositoryMock.VerifyNoOtherCalls();
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantRepositoryGetAll(params CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.GetAll()).Returns(combatantEntities).Verifiable();
        }
        
        private void SetupCombatantRepositoryGet(byte id)
        { 
            _combatantRepositoryMock.Setup(library => library.Get(id)).Returns(_combatantEntity).Verifiable();
        }
        
        private void SetupCombatantRepositoryContains(byte id)
        { 
            _combatantRepositoryMock.Setup(library => library.Contains(id)).Returns(true).Verifiable();
        }

        private void SetupAbilityRepositoryContains(byte combatantID)
        {
            _abilityRepositoryMock.Setup(library => library.Contains(combatantID)).Returns(true).Verifiable();
        }
        
        private void SetupAbilityRepositoryGet(byte combatantID, CombatantAbilityEntity combatantAbilityEntity)
        {
            _abilityRepositoryMock.Setup(library => library.Get(combatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void SetupAbilityRepositoryGetAll(byte combatantID, params CombatantAbilityEntity[] abilityEntities)
        {
            _abilityRepositoryMock.Setup(library => library.GetAll(combatantID)).Returns(abilityEntities).Verifiable();
        }

        private void VerifyAbilityRepositoryContainsCalled(byte combatantID, Times times)
        {
            _abilityRepositoryMock.Verify(library => library.Contains(combatantID), times);
        }

        private void VerifyQueueCalled(double tickTime, Times times)
        {
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<BasicAttackEvent>(), It.Is<double>(number => Math.Abs(number - tickTime) < 0.0001)), times);
        }

        [Test]
        public void Positive_EnqueueInitial_NoCreatedAbility_NoEnqueue()
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, _attackerStats, _attackerCreation.Information, []);
            CombatantEntity combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, combatantCreation);
            SetupCombatantRepositoryGetAll(combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyAbilityRepositoryContainsCalled(1, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupAbilityRepositoryContains(_combatantEntity.CombatantID);
            SetupAbilityRepositoryGetAll(_combatantEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyQueueCalled(1.1d, Times.Once());
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
            VerifyQueueCalled(1.1d, Times.Exactly(5));
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
            VerifyQueueCalled(0.1d, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            SetupCombatantRepositoryGetAll();
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<BasicAttackEvent>(), It.IsAny<double>()), Times.Never());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_EnqueueInitial_ZeroSpeed_Throws()
        {
            CombatantEntity zeroSpeedEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, true, _attackerStats with { Speed = 0 });
            SetupCombatantRepositoryGetAll(zeroSpeedEntity);
            
            SetupAbilityRepositoryContains(zeroSpeedEntity.CombatantID);
            SetupAbilityRepositoryGetAll(zeroSpeedEntity.CombatantID, _combatantAbilityEntity);
            SetupCombatantRepositoryGetAll(zeroSpeedEntity);
            
            Assert.Throws<NumberZeroException>(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            VerifyAbilityRepositoryContainsCalled(zeroSpeedEntity.CombatantID, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_EnqueueInitial_ZeroCooldown_Throws()
        {
            CombatantAbilityEntity zeroCooldownEntity = TestCombatantAbilityEntityFactory.Create(1, AbilityType.BASIC_ATTACK);
            zeroCooldownEntity.AddComponent(new CooldownComponent { Cooldown = 0 });
            
            SetupAbilityRepositoryContains(zeroCooldownEntity.CombatantID);
            SetupAbilityRepositoryGetAll(zeroCooldownEntity.CombatantID, zeroCooldownEntity);
            SetupCombatantRepositoryGetAll(_combatantEntity);
            
            Assert.Throws<NumberZeroException>(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            VerifyAbilityRepositoryContainsCalled(1, Times.Once());
            VerifyMocks();
        }

        
        [Test]
        public void Positive_EnqueueAttack_QueuesAttack()
        {
            SetupCombatantRepositoryContains(_combatantAbilityEntity.CombatantID);
            SetupCombatantRepositoryGet(_combatantAbilityEntity.CombatantID);
            SetupAbilityRepositoryContains(_combatantAbilityEntity.CombatantID);
            SetupAbilityRepositoryGet(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueAttack(1d, _combatantAbilityEntity.CombatantID, AbilityType.BASIC_ATTACK));
            
            VerifyQueueCalled(1.1d, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_EnqueueAttack_CombatantRepositoryNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _basicAttackScheduler.EnqueueAttack(1d, _combatantEntity.CombatantID, AbilityType.BASIC_ATTACK));
            
            _combatantRepositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_EnqueueAttack_AbilityRepositoryNotFound_Throws()
        {
            SetupCombatantRepositoryContains(_combatantAbilityEntity.CombatantID);
            
            Assert.Throws<NotFoundException<byte>>(() => _basicAttackScheduler.EnqueueAttack(1d, _combatantEntity.CombatantID, AbilityType.BASIC_ATTACK));
            
            _combatantRepositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once());
            _abilityRepositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once());
            VerifyMocks();
        }
    }
}