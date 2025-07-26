using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class ScheduleControlFactory : IScheduleControlFactory
    {
        public ScheduleControl Create(ControlAction controlAction)
        {
            return new ScheduleControl
            {
                ControlAction = controlAction
            };
        }
    }
}