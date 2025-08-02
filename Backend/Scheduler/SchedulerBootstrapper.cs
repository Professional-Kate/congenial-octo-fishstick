using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Scheduler.Core.Mediator;
using Scheduler.Core.Register;
using Scheduler.Factory;
using Scheduler.Flows.Control;
using Scheduler.Flows.Control.Runner;
using Scheduler.Types;

namespace Scheduler
{
    public class SchedulerBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ITaskErrorDTOFactory taskErrorDTOFactory = new TaskErrorDTOFactory(baseErrorFactory);
            IDispatchOne<ScheduledTaskErrorDTO> errorDTODispatcher = new ManagedDispatcher<ScheduledTaskErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            IScheduleReader scheduleReader = new ScheduleRegister(uniqueAssertion, foundAssertion, objectNullAssertion);
            IScheduleRunnerMediator scheduleRunnerMediator = new ScheduleRunnerMediator(scheduleReader, errorDTODispatcher, taskErrorDTOFactory, collectionAssertion);

            IManagedTimer threadingManagedTimer = new ThreadingTimer(scheduleRunnerMediator);
            IScheduleRunner scheduleRunner = new ScheduleRunner(threadingManagedTimer);
        }
    }
}