using IdelPog.Common.Enums;

namespace Console.Commands.Domains.Arguments
{
    public readonly record struct ScheduleControlArguments
    {
        public required ControlAction ControlAction { get; init; }
    }
}