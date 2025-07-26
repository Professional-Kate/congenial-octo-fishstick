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
            IAssertNonDuplicate assertNonDuplicate = new AssertNonDuplicate(throwHandler);
            IAssertFound assertFound = new AssertFound(throwHandler);
            IAssertNotNull assertNotNull = new AssertNotNull(throwHandler);
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(throwHandler);

            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();
            ITaskErrorDTOFactory taskErrorDTOFactory = new TaskErrorDTOFactory(errorDTOFactory);
            IDispatchOne<ScheduledTaskErrorDTO> errorDTODispatcher =
                new ManagedDispatcher<ScheduledTaskErrorDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);

            IScheduleReader scheduleReader = new ScheduleRegister(assertNonDuplicate, assertFound, assertNotNull);
            IScheduleMediator scheduleMediator = new ScheduleMediator(scheduleReader, errorDTODispatcher, taskErrorDTOFactory, assertCollectionNotEmpty);

            IManagedTimer threadingManagedTimer = new ThreadingTimer(scheduleMediator.RunUpdate);
            IScheduleRunner scheduleRunner = new ScheduleRunner(threadingManagedTimer);
            IScheduleController scheduleController = new ScheduleController(scheduleRunner);
            ISingleListener<ScheduleControl> scheduleControlListener = new ScheduleControlListener(scheduleController);

            bufferMessenger.Subscribe(scheduleControlListener);
        }
    }
}