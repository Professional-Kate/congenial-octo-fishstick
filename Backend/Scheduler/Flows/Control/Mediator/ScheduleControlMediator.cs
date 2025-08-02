using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Listeners.Single;
using Scheduler.Flows.Control.Runner;

namespace Scheduler.Flows.Control.Mediator
{
    public class ScheduleControlMediator : ISingleMediator<ScheduleControl>
    {
        private readonly IScheduleRunner  _scheduleRunner;

        public ScheduleControlMediator(IScheduleRunner scheduleRunner)
        {
            _scheduleRunner = scheduleRunner;
        }

        public void HandleMessage(ScheduleControl scheduleControl)
        {
            switch (scheduleControl.ControlAction)
            {
                case ControlAction.START:
                    StartSchedule();
                    break;
                case ControlAction.STOP: 
                    StopSchedule();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheduleControl), scheduleControl, "ControlAction not supported.");
            }
        }

        private void StartSchedule()
        {
            _scheduleRunner.StartSchedule(TimeSpan.FromSeconds(5));
        }

        private void StopSchedule()
        {
            _scheduleRunner.StopSchedule();
        }
    }
}