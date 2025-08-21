using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Schedule
{
    public readonly record struct ScheduleControlArguments
    {
        public required ControlAction ControlAction { get; init; }
    }
}