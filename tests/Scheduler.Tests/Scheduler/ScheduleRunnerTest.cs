using Moq;
using Scheduler.Mediator;
using Scheduler.Runner;

namespace Scheduler.Tests.Scheduler
{
    [TestFixture]
    public class ScheduleRunnerTest
    {
        private IScheduleRunner _scheduleRunner;
        private Mock<IScheduleMediator>  _scheduleMediatorMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _scheduleMediatorMock = new Mock<IScheduleMediator>();
            _scheduleRunner = new ScheduleRunner(_scheduleMediatorMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _scheduleMediatorMock.Reset();
        }

        [Test]
        public void Positive_StartSchedule_InvokesMediatorAfterTimeSpan()
        {
            ManualResetEventSlim resetEvent = new(false);
            
            _scheduleMediatorMock.Setup(library => library.RunUpdate()).Callback(() => resetEvent.Set());
            
            _scheduleRunner.StartSchedule(TimeSpan.FromMilliseconds(1));
            
            bool wasCalled = resetEvent.Wait(TimeSpan.FromSeconds(1));
            Assert.That(wasCalled, Is.True);
            _scheduleMediatorMock.Verify(library => library.RunUpdate(), Times.Once);
        }

        [Test]
        public void Positive_StopSchedule_DoesNotInvokeMediatorAfterTimeSpan()
        {
            ManualResetEventSlim resetEvent = new(false);
            
            _scheduleMediatorMock.Setup(library => library.RunUpdate()).Callback(() => resetEvent.Set());
            
            _scheduleRunner.StopSchedule();
            
            bool wasCalled = resetEvent.Wait(TimeSpan.FromSeconds(1));
            Assert.That(wasCalled, Is.False);
            _scheduleMediatorMock.Verify(library => library.RunUpdate(), Times.Never);
        }
    }
}