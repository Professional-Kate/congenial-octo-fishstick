using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface IScheduleControlFactory
    {
        public ScheduleControl Create(ControlAction controlAction);
    }
}