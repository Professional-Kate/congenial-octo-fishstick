using IdelPog.Core.Contracts.Error;

namespace IdelPog.Core.Scheduler.Error
{
    public readonly record struct ScheduledTaskError
    {
        public required Type TaskType { get; init; }
        public required BaseError BaseError { get; init; }
    }
}