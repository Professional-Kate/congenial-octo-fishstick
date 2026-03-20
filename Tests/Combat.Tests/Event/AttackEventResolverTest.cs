using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime.System.Interface;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class AttackEventResolverTest
    {
        private AttackEventResolver _attackEventResolver;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<IAttackScheduler> _attackSchedulerMock;

        private AttackEvent _attackEvent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystemMock = new Mock<IDamageSystem>();
            _attackSchedulerMock = new Mock<IAttackScheduler>();
            
            _attackEventResolver = new AttackEventResolver(_damageSystemMock.Object, _attackSchedulerMock.Object);

            _attackEvent = new AttackEvent { AttackerID = 0, Tick = 0 };
        }

        private void VerifyDamageSystem()
        {
            _damageSystemMock.Verify(library => library.ApplyDamage(_attackEvent.AttackerID), Times.Once);
        }

        private void VerifyAttackScheduler()
        {
            _attackSchedulerMock.Verify(library => library.EnqueueAttack(_attackEvent.Tick, _attackEvent.AttackerID), Times.Once);
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage_EnqueuesAttack()
        { 
            Assert.DoesNotThrow(() => _attackEventResolver.ResolveEvent(_attackEvent.Tick, _attackEvent.AttackerID));

            VerifyDamageSystem();
            VerifyAttackScheduler();
        }
    }
}