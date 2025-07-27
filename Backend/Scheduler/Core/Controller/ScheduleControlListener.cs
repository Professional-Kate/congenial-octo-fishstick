using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Listeners;

namespace Scheduler.Core.Controller
{
    public class ScheduleControlListener : ISingleListener<ScheduleControl>
    {
        private readonly IScheduleController _scheduleController;

        public ScheduleControlListener(IScheduleController scheduleController)
        {
            _scheduleController = scheduleController;
        }

        public Type ListenerType => typeof(ScheduleControl);

        public void Handle(ScheduleControl harvestNode)
        {
            switch (harvestNode.ControlAction)
            {
                case ControlAction.START:
                    _scheduleController.StartSchedule();
                    break;
                case ControlAction.STOP:
                    _scheduleController.StopSchedule();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(harvestNode));
            }
        }
    }
}