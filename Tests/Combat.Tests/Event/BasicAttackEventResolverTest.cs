using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class BasicAttackEventResolverTest
    {
        private BasicAttackEventResolver _basicAttackEventResolver;
        private Mock<IEntityDamageMediator> _damageSystemMock;
        private Mock<IBasicAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private BasicAttackEvent _basicAttackEvent;
        private CombatantEntity _combatantEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IEntityDamageMediator>();
            _attackSchedulerMock = new Mock<IBasicAttackScheduler>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _basicAttackEventResolver = new BasicAttackEventResolver(_damageSystemMock.Object, _attackSchedulerMock.Object, _combatantRepositoryMock.Object, new FoundAssertion());

            _basicAttackEvent = new BasicAttackEvent { AttackerID = 0, Tick = 0 };

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
            _damageSystemMock.Verify(library => library.ApplyDamage(_basicAttackEvent.AttackerID, SkillType.BASIC_ATTACK), Times.Once);
        }

        private void VerifyEventEnqueued()
        {
            _attackSchedulerMock.Verify(library => library.EnqueueAttack(_basicAttackEvent.Tick, _basicAttackEvent.AttackerID), Times.Once);
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
            SetupRepositoryContains(_basicAttackEvent.AttackerID);
            SetupRepositoryGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _basicAttackEventResolver.ResolveEvent(_basicAttackEvent.Tick, _basicAttackEvent.AttackerID));

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
            
            Assert.DoesNotThrow(() => _basicAttackEventResolver.ResolveEvent(_basicAttackEvent.Tick, deadEntity.CombatantID));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _basicAttackEventResolver.ResolveEvent(_basicAttackEvent.Tick, _basicAttackEvent.AttackerID));
            
            _combatantRepositoryMock.Verify(library => library.Contains(_basicAttackEvent.AttackerID), Times.Once);
            VerifyMocks();
        }
    }
}