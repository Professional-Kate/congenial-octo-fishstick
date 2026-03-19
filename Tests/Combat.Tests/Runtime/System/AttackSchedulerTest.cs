using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AttackSchedulerTest
    {
        private AttackScheduler _attackScheduler;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _combatantEntity;
        private StatCard _attackerStats;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatQueueMock = new Mock<ICombatQueue>();
            _damageSystemMock = new Mock<IDamageSystem>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            
            _attackScheduler = new AttackScheduler(_combatQueueMock.Object, _damageSystemMock.Object, new NumberAssertion(), _combatantRepositoryMock.Object, new FoundAssertion());

            _attackerStats = new StatCard { Health = 100, Attack = 10, Speed = 10 };
            _combatantEntity = new CombatantEntity(_repositoryAsserter, _attackerStats) { IsFriendly = true, CombatantID = 0 };
        }

        [SetUp]
        public void Setup()
        {
            _combatQueueMock.Reset();
            _damageSystemMock.Reset();
        }

        private void SetupRepositoryGetAll(params CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.GetAll()).Returns(combatantEntities).Verifiable();
        }
        
        private void SetupRepositoryGet(byte id)
        { 
            _combatantRepositoryMock.Setup(library => library.Get(id)).Returns(_combatantEntity).Verifiable();
        }

        private void VerifyQueueCalled(double tickTime, Times times)
        {
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<AttackEvent>(), It.Is<double>(number => Math.Abs(number - tickTime) < 0.0001)), times);
        }

        private void VerifyQueue()
        {
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _attackScheduler.EnqueueInitial(1d));

            VerifyQueueCalled(1.1d, Times.Once());
            VerifyQueue();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesMultipleAttackEvent()
        {
            SetupRepositoryGetAll(_combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity);
            
            Assert.DoesNotThrow(() => _attackScheduler.EnqueueInitial(1d));

            VerifyQueueCalled(1.1d, Times.Exactly(5));
            VerifyQueue();
        }
        
        [Test]
        public void Positive_EnqueueInitial_ZeroTick()
        {
            SetupRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _attackScheduler.EnqueueInitial(0d));
            
            VerifyQueueCalled(0.1d, Times.Once());
            VerifyQueue();
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            SetupRepositoryGetAll();
            
            Assert.DoesNotThrow(() => _attackScheduler.EnqueueInitial(1d));
            
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<AttackEvent>(), It.IsAny<double>()), Times.Never());
            VerifyQueue();
        }
        
        [Test]
        public void Negative_EnqueueInitial_ZeroSpeed_Throws()
        {
            CombatantEntity zeroSpeedEntity = new(_repositoryAsserter, _attackerStats with { Speed = 0 }) { IsFriendly = true, CombatantID = 0 };
            SetupRepositoryGetAll(zeroSpeedEntity);
            
            Assert.Throws<NumberZeroException>(() => _attackScheduler.EnqueueInitial(1d));
            
            VerifyQueue();
        }

        [Test]
        public void Positive_EnqueueAttack_QueuesAttack()
        { 
            SetupRepositoryGet(0);
            _combatantRepositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            
            Assert.DoesNotThrow(() => _attackScheduler.EnqueueAttack(1d, 0));
            
            VerifyQueueCalled(1.1d, Times.Once());
            VerifyQueue();
        }

        [Test]
        public void Negative_EnqueueAttack_IDNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _attackScheduler.EnqueueAttack(1d, 0));
            
            VerifyQueue();
        }
    }
}