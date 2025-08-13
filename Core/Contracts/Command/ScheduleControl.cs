using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct ScheduleControl
    {
        public required ControlAction ControlAction { get; init; }
    }
}