using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Scheduler.Core.Register;

namespace Scheduler.Tests.Scheduler
{
    [TestFixture]
    public class ScheduleRegisterTest
    {
        private IScheduleRegister _scheduleRegister;
        private IScheduleReader _scheduleReader;

        [SetUp]
        public void Setup()
        {
            ScheduleRegister scheduleRegister = new(new UniqueAssertion(new ThrowHandler()), new FoundAssertion(new ThrowHandler()),
                new ObjectNullAssertion(new ThrowHandler()));

            _scheduleReader = scheduleRegister;
            _scheduleRegister = scheduleRegister;
        }

        [Test]
        public void Positive_Register_CallsRepositoryAdd()
        {
            TestScheduledTask task = new();

            Assert.DoesNotThrow(() => _scheduleRegister.Register(task));
        }

        [Test]
        public void Negative_Register_DuplicateTypes_Throws()
        {
            TestScheduledTask task = new();

            _scheduleRegister.Register(task);
            Assert.Throws<DuplicateItemException>(() => _scheduleRegister.Register(task));
        }

        [Test]
        public void Positive_Unregister_CallsRepositoryAdd()
        {
            TestScheduledTask task = new();

            _scheduleRegister.Register(task);
            Assert.DoesNotThrow(() => _scheduleRegister.Unregister(task));
        }

        [Test]
        public void Negative_Unregister_TypeNotFound_Throws()
        {
            TestScheduledTask task = new();
            
            NotFoundException<IScheduledTask> exception = Assert.Throws<NotFoundException<IScheduledTask>>(() => _scheduleRegister.Unregister(task));
            Assert.That(exception.Key, Is.EqualTo(task));
        }

        [Test]
        public void Negative_AnyAction_NullTask_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _scheduleRegister.Unregister(null!));
            Assert.Throws<ArgumentNullException>(() => _scheduleRegister.Register(null!));
        }

        [Test]
        public void Positive_GetScheduledTasks_ReturnsAllTasks()
        {
            TestScheduledTask task = new();
            TestThrowingTask throwingTask = new();

            _scheduleRegister.Register(task);
            _scheduleRegister.Register(throwingTask);

            IReadOnlyList<IScheduledTask> tasks = _scheduleReader.GetScheduledTasks();
            Assert.That(tasks, Has.Count.EqualTo(2));
        }

        [Test]
        public void Positive_GetScheduledTasks_ReturnsZeroTasks()
        {
            IReadOnlyList<IScheduledTask> tasks = _scheduleReader.GetScheduledTasks();
            Assert.That(tasks, Has.Count.EqualTo(0));
        }
    }
}