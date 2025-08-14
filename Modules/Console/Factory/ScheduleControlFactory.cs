using IdelPog.Console.Factory.Interface;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Factory
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