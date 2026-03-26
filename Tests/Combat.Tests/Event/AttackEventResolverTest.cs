using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class AttackEventResolverTest
    {
        private AttackEventResolver _attackEventResolver;
        private Mock<IEntityDamageSystem> _damageSystemMock;
        private Mock<IAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private AttackEvent _attackEvent;
        private CombatantEntity _combatantEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IEntityDamageSystem>();
            _attackSchedulerMock = new Mock<IAttackScheduler>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _attackEventResolver = new AttackEventResolver(_damageSystemMock.Object, _attackSchedulerMock.Object, _combatantRepositoryMock.Object, new FoundAssertion());

            _attackEvent = new AttackEvent { AttackerID = 0, Tick = 0 };

            _combatantEntity = CombatantEntityFactory.CreateCombatantEntity(0);
        }

        [SetUp]
        public void Setup()
        {
            _damageSystemMock.Reset();
            _attackSchedulerMock.Reset();
            _combatantRepositoryMock.Reset();
        }

        private void VerifyDamageApplied()
        {
            _damageSystemMock.Verify(library => library.ApplyDamage(_attackEvent.AttackerID), Times.Once);
        }

        private void VerifyEventEnqueued()
        {
            _attackSchedulerMock.Verify(library => library.EnqueueAttack(_attackEvent.Tick, _attackEvent.AttackerID), Times.Once);
        }

        private void SetupRepositoryContains(byte combatantID)
        {
            _combatantRepositoryMock.Setup(library => library.Contains(combatantID)).Returns(true).Verifiable();
        }

        private void SetupRepositoryGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private void VerifyMocks()
        {
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
            
            _attackSchedulerMock.Verify();
            _attackSchedulerMock.VerifyNoOtherCalls();
            
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage_EnqueuesAttack()
        {
            SetupRepositoryContains(_attackEvent.AttackerID);
            SetupRepositoryGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _attackEventResolver.ResolveEvent(_attackEvent.Tick, _attackEvent.AttackerID));

            VerifyDamageApplied();
            VerifyEventEnqueued();
            VerifyMocks();
        }

        [Test]
        public void Positive_ResolveEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = CombatantEntityFactory.CreateCombatantEntity(1);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepositoryContains(deadEntity.CombatantID);
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _attackEventResolver.ResolveEvent(_attackEvent.Tick, deadEntity.CombatantID));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _attackEventResolver.ResolveEvent(_attackEvent.Tick, _attackEvent.AttackerID));
            
            _combatantRepositoryMock.Verify(library => library.Contains(_attackEvent.AttackerID), Times.Once);
            VerifyMocks();
        }
    }
}