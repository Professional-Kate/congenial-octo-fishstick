using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Domain.Argument
{
    public readonly record struct ScheduleControlArguments
    {
        public required ControlAction ControlAction { get; init; }
    }
}