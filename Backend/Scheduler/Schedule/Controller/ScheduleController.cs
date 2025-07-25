using Scheduler.Runner;

namespace Scheduler.Controller
{
    public class ScheduleController : IScheduleController
    {
        private IScheduleRunner  _scheduleRunner;

        public ScheduleController(IScheduleRunner scheduleRunner)
        {
            _scheduleRunner = scheduleRunner;
        }

        public void StartSchedule()
        {
            // TODO: replace this with a StatProvider
            _scheduleRunner.StartSchedule(TimeSpan.FromSeconds(1));
        }

        public void StopSchedule()
        {
            _scheduleRunner.StopSchedule();
        }
    }
}