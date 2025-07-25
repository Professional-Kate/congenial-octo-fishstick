using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;
using Scheduler.Factory;
using Scheduler.Mediator;
using Scheduler.Register;
using Scheduler.Types;

namespace Scheduler.Tests.Scheduler
{
    [TestFixture]
    public class ScheduleMediatorTest
    {
        private IScheduleMediator _scheduleMediator;
        private Mock<IScheduleReader>  _scheduleReaderMock;
        private Mock<IDispatchOne<ScheduledTaskErrorDTO>> _dispatcherMock;
        private Mock<ITaskErrorDTOFactory>  _taskErrorDTOFactoryMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _scheduleReaderMock = new Mock<IScheduleReader>();
            _dispatcherMock = new Mock<IDispatchOne<ScheduledTaskErrorDTO>>();
            _taskErrorDTOFactoryMock = new Mock<ITaskErrorDTOFactory>();
            _scheduleMediator = new ScheduleMediator(_scheduleReaderMock.Object, _dispatcherMock.Object, _taskErrorDTOFactoryMock.Object, new AssertCollectionNotEmpty(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _scheduleReaderMock.Reset();
            _dispatcherMock.Reset();
            _taskErrorDTOFactoryMock.Reset();
        }

        [Test]
        public void Positive_RunUpdate_InvokesMultipleTasks()
        {
            IReadOnlyList<IScheduledTask> tasks = new List<IScheduledTask> { new TestScheduledTask(), new TestScheduledTask(), new TestScheduledTask() };

            _scheduleReaderMock.Setup(library => library.GetScheduledTasks())
                .Returns(tasks);
            
            _scheduleMediator.RunUpdate();
            
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<ScheduledTaskErrorDTO>()), Times.Never);
            _scheduleReaderMock.Verify(library => library.GetScheduledTasks(), Times.Once);
            _taskErrorDTOFactoryMock.Verify(library => library.Create(It.IsAny<Exception>(), typeof(TestScheduledTask)), Times.Never);
            
            foreach (IScheduledTask scheduledTask in tasks)
            {
                if (scheduledTask is not TestScheduledTask testScheduledTask) continue;

                Assert.Multiple(() =>
                {
                    Assert.That(testScheduledTask.WasCalled, Is.True);
                    Assert.That(testScheduledTask.AmountCalled, Is.EqualTo(1));
                });
            }
        }

        [Test]
        public void Positive_RunUpdate_OneTaskThrows_Suppresses_DispatchesOneErrorDTO()
        {
            IReadOnlyList<IScheduledTask> tasks = new List<IScheduledTask> { new TestScheduledTask(), new TestThrowingTask(), new TestScheduledTask(), new TestScheduledTask() };
            
            _scheduleReaderMock.Setup(library => library.GetScheduledTasks())
                .Returns(tasks);
            
            Assert.DoesNotThrow(() => _scheduleMediator.RunUpdate());
            
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<ScheduledTaskErrorDTO>()), Times.Once);
            _scheduleReaderMock.Verify(library => library.GetScheduledTasks(), Times.Once);
            _taskErrorDTOFactoryMock.Verify(library => library.Create(It.IsAny<Exception>(), It.IsAny<Type>()), Times.Once);

            foreach (IScheduledTask scheduledTask in tasks)
            {
                switch (scheduledTask)
                {
                    case TestScheduledTask testScheduledTask:
                            Assert.That(testScheduledTask.WasCalled, Is.True);
                            Assert.That(testScheduledTask.AmountCalled, Is.EqualTo(1));
                            break;
                    case TestThrowingTask testThrowingTask:
                        Assert.That(testThrowingTask.AmountCalled, Is.EqualTo(1));
                        break;
                }
            }
        }

        [Test]
        public void Positive_RunUpdate_MultipleTasksThrow_SuppressesEach_DispatchesMultipleErrorDTO()
        {
            IReadOnlyList<IScheduledTask> tasks = new List<IScheduledTask> { new TestThrowingTask(), new TestThrowingTask(), new TestThrowingTask(), new TestThrowingTask() };
            
            _scheduleReaderMock.Setup(library => library.GetScheduledTasks())
                .Returns(tasks);
            
            Assert.DoesNotThrow(() => _scheduleMediator.RunUpdate());
            
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<ScheduledTaskErrorDTO>()), Times.Exactly(tasks.Count));
            _scheduleReaderMock.Verify(library => library.GetScheduledTasks(), Times.Once);
            _taskErrorDTOFactoryMock.Verify(library => library.Create(It.IsAny<Exception>(), typeof(TestThrowingTask)), Times.Exactly(tasks.Count));
        }

        [Test]
        public void Negative_RunUpdate_NoTasksToRun_Throws()
        {
            _scheduleReaderMock.Setup(library => library.GetScheduledTasks())
                .Returns(new List<IScheduledTask>());
            
            Assert.Throws<EmptyCollectionException>(() => _scheduleMediator.RunUpdate());
            _scheduleReaderMock.Verify(library => library.GetScheduledTasks(), Times.Once);
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<ScheduledTaskErrorDTO>()), Times.Never);
            _taskErrorDTOFactoryMock.Verify(library => library.Create(It.IsAny<Exception>(), typeof(TestScheduledTask)), Times.Never);
        }
    }
}