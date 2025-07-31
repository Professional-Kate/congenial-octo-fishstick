using Moq;
using Scheduler.Flows.Control;
using Scheduler.Flows.Control.Runner;

namespace Scheduler.Tests.Scheduler
{
    [TestFixture]
    public class ScheduleRunnerTest
    {
        private IScheduleRunner _scheduleRunner;
        private Mock<IManagedTimer> _managedTimerMock;

        [SetUp]
        public void Setup()
        {
            _managedTimerMock = new Mock<IManagedTimer>();
            _scheduleRunner = new ScheduleRunner(_managedTimerMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _managedTimerMock.Reset();
        }

        [Test]
        public void Positive_StartSchedule_InvokesTimerStart()
        {
            _scheduleRunner.StartSchedule(TimeSpan.FromMilliseconds(1));

            _managedTimerMock.Verify(library => library.Start(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1)), Times.Once);
            _managedTimerMock.Verify(library => library.Stop(), Times.Never);
        }

        [Test]
        public void Positive_StopSchedule_InvokesTimerStop()
        {
            _scheduleRunner.StopSchedule();

            _managedTimerMock.Verify(library => library.Start(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()), Times.Never);
            _managedTimerMock.Verify(library => library.Stop(), Times.Once);
        }
    }
}