using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Factory.Interface
{
    public interface IScheduleControlFactory
    {
        public ScheduleControl Create(ControlAction controlAction);
    }
}