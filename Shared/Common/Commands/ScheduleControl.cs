using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct ScheduleControl
    {
        public required ControlAction ControlAction { get; init; }
    }
}