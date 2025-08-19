using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Scheduler;
using Moq;

namespace IdelPog.Core.Tests.Scheduler
{
    [TestFixture]
    public class TickEmitterTest
    {
        private ITickEmitter _tickEmitter;
        private Mock<IDispatchOne<ScheduleTick>> _tickDispatcherMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tickDispatcherMock = new Mock<IDispatchOne<ScheduleTick>>();
            _tickEmitter = new TickEmitter(_tickDispatcherMock.Object);
        }

        [Test]
        public void Positive_RunUpdate_DispatchesUpdate()
        {
            _tickEmitter.RunUpdate();
            
            _tickDispatcherMock.Verify(library => library.Dispatch(It.IsAny<ScheduleTick>()), Times.Once);
        }
    }
}