using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class BasicAttackSchedulerTest
    {
        private BasicAttackScheduler _basicAttackScheduler;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IEntityDamageMediator> _damageSystemMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        
        private CombatantEntity _combatantEntity;
        private StatCard _attackerStats;
        private CombatantCreation _attackerCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatQueueMock = new Mock<ICombatQueue>();
            _damageSystemMock = new Mock<IEntityDamageMediator>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _basicAttackScheduler = new BasicAttackScheduler(_combatQueueMock.Object, new NumberAssertion(), _combatantRepositoryMock.Object, new FoundAssertion());

            _attackerStats = new StatCard { Health = 100, Attack = 10, Speed = 10 };
            _attackerCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, _attackerStats);
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _attackerCreation);
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
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<BasicAttackEvent>(), It.Is<double>(number => Math.Abs(number - tickTime) < 0.0001)), times);
        }

        private void VerifyQueue()
        {
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
        }

        // [Test]
        public void Positive_EnqueueInitial_NoMatchingComponent_NoEnqueue()
        {
            // TODO: need to add another SkillComponent
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, _attackerStats, _attackerCreation.Information, []);
            CombatantEntity combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, combatantCreation);
            SetupRepositoryGetAll(combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyQueue();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesSingleAttackEvent()
        {
            SetupRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyQueueCalled(1.1d, Times.Once());
            VerifyQueue();
        }
        
        [Test]
        public void Positive_EnqueueInitial_EnqueuesMultipleAttackEvent()
        {
            SetupRepositoryGetAll(_combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity, _combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));

            VerifyQueueCalled(1.1d, Times.Exactly(5));
            VerifyQueue();
        }
        
        [Test]
        public void Positive_EnqueueInitial_ZeroTick()
        {
            SetupRepositoryGetAll(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(0d));
            
            VerifyQueueCalled(0.1d, Times.Once());
            VerifyQueue();
        }

        [Test]
        public void Positive_EnqueueInitial_NoEntities_NoAction()
        {
            SetupRepositoryGetAll();
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            _combatQueueMock.Verify(library => library.Enqueue(It.IsAny<BasicAttackEvent>(), It.IsAny<double>()), Times.Never());
            VerifyQueue();
        }
        
        [Test]
        public void Negative_EnqueueInitial_ZeroSpeed_Throws()
        {
            CombatantEntity zeroSpeedEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, true, _attackerStats with { Speed = 0 });
            SetupRepositoryGetAll(zeroSpeedEntity);
            
            Assert.Throws<NumberZeroException>(() => _basicAttackScheduler.EnqueueInitial(1d));
            
            VerifyQueue();
        }

        [Test]
        public void Positive_EnqueueAttack_QueuesAttack()
        { 
            SetupRepositoryGet(0);
            _combatantRepositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            
            Assert.DoesNotThrow(() => _basicAttackScheduler.EnqueueAttack(1d, 0));
            
            VerifyQueueCalled(1.1d, Times.Once());
            VerifyQueue();
        }

        [Test]
        public void Negative_EnqueueAttack_IDNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _basicAttackScheduler.EnqueueAttack(1d, 0));
            
            VerifyQueue();
        }
    }
}