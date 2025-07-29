using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Scheduler.Core;
using Scheduler.Core.Controller;
using Scheduler.Core.Mediator;
using Scheduler.Core.Register;
using Scheduler.Core.Runner;
using Scheduler.Factory;
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

            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();
            ITaskErrorDTOFactory taskErrorDTOFactory = new TaskErrorDTOFactory(errorDTOFactory);
            IDispatchOne<ScheduledTaskErrorDTO> errorDTODispatcher =
                new ManagedDispatcher<ScheduledTaskErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            IScheduleReader scheduleReader = new ScheduleRegister(uniqueAssertion, foundAssertion, objectNullAssertion);
            IScheduleMediator scheduleMediator = new ScheduleMediator(scheduleReader, errorDTODispatcher, taskErrorDTOFactory, collectionAssertion);

            IManagedTimer threadingManagedTimer = new ThreadingTimer(scheduleMediator.RunUpdate);
            IScheduleRunner scheduleRunner = new ScheduleRunner(threadingManagedTimer);
            IScheduleController scheduleController = new ScheduleController(scheduleRunner);
            ISingleListener<ScheduleControl> scheduleControlListener = new ScheduleControlListener(scheduleController);

            bufferMessenger.Subscribe(scheduleControlListener);
        }
    }
}